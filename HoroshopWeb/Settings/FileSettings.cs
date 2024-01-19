using System.ComponentModel.DataAnnotations;

namespace HoroshopWeb.Settings;

public class FileSettings
{
    [Required]
    public required string DataFolder { get; set; }
}
