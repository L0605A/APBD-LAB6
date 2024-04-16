using System.ComponentModel.DataAnnotations;

namespace APBD6.Models.DTOs;

public class DTOAnimal
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; }
    [MaxLength(200)]
    public string? Desc { get; set; }
    [Required]
    [MaxLength(200)]
    public string Category { get; set; }
    [Required]
    [MaxLength(200)]
    public string Area { get; set; }
}