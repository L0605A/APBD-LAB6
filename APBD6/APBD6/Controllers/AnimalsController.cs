using APBD6.Models;
using APBD6.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace APBD6.Controllers;

[ApiController]
[Route("api/animals")]

public class AnimalsController : ControllerBase
{

    private readonly IConfiguration _configuration;

    public AnimalsController(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    [HttpGet]
    public IActionResult GetAnimals([FromQuery] string orderBy = "name")
    {
        
        orderBy = orderBy?.ToLower();
        
        if (orderBy != "name" && orderBy != "description" && orderBy != "category" && orderBy != "area")
        {
            
            orderBy = "name";
        }

        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        connection.Open();

        using SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = $"SELECT * FROM Animal ORDER BY {orderBy} ASC";

        var reader = command.ExecuteReader();

        List<Animal> animals = new List<Animal>();

        int idAnimalOrdinal = reader.GetOrdinal("IdAnimal");
        int nameOrdinal = reader.GetOrdinal("Name");
        int descOrdinal = reader.GetOrdinal("Description");
        int categoryOrdinal = reader.GetOrdinal("Category");
        int areaOrdinal = reader.GetOrdinal("Area");

        while (reader.Read())
        {
            animals.Add(new Animal()
            {
                IdAnimal = reader.GetInt32(idAnimalOrdinal),
                Name = reader.GetString(nameOrdinal),
                Desc = reader.GetString(descOrdinal),
                Category = reader.GetString(categoryOrdinal),
                Area = reader.GetString(areaOrdinal),
            });
        }

        return Ok(animals);
    }


    [HttpPost]
    public IActionResult AddAnimal(DTOAnimal addAnimal)
    {
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        connection.Open();

        using SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = "INSERT INTO Animal VALUES (@animalName, @animalDesc, @animalCategory, @animalArea)";
        command.Parameters.AddWithValue("@animalName", addAnimal.Name);
        command.Parameters.AddWithValue("@animalDesc", addAnimal.Desc);
        command.Parameters.AddWithValue("@animalCategory", addAnimal.Category);
        command.Parameters.AddWithValue("@animalArea", addAnimal.Area);

        command.ExecuteReader();
        
        return Created();
    }
    
    [HttpPut("{idAnimal}")]
    public IActionResult UpdateAnimal(int idAnimal, [FromBody] DTOAnimal updatedAnimalData)
    {
        if (updatedAnimalData == null)
        {
            return BadRequest("Updated animal data is required.");
        }

        using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default")))
        {
            connection.Open();

            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = connection;
                command.CommandText = "UPDATE Animal " +
                                      "SET Name = @animalName, " +
                                      "    Description = @animalDesc, " +
                                      "    Category = @animalCategory, " +
                                      "    Area = @animalArea " +
                                      "WHERE IdAnimal = @idAnimal";
                command.Parameters.AddWithValue("@animalName", updatedAnimalData.Name);
                command.Parameters.AddWithValue("@animalDesc", updatedAnimalData.Desc);
                command.Parameters.AddWithValue("@animalCategory", updatedAnimalData.Category);
                command.Parameters.AddWithValue("@animalArea", updatedAnimalData.Area);
                command.Parameters.AddWithValue("@idAnimal", idAnimal);

                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected == 0)
                {
                    return NotFound("Animal with the specified IdAnimal was not found.");
                }
            }
        }

        return Ok();
    }

    [HttpDelete("{idAnimal}")]
    public IActionResult DeleteAnimal(int idAnimal)
    {
        using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default")))
        {
            connection.Open();

            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = connection;
                command.CommandText = "DELETE FROM Animal WHERE IdAnimal = @idAnimal";
                command.Parameters.AddWithValue("@idAnimal", idAnimal);

                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected == 0)
                {
                    return NotFound("Animal with the specified IdAnimal was not found.");
                }
            }
        }

        return Ok();
    }

    
}