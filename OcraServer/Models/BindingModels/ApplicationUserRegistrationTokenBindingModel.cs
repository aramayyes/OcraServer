using System;
using System.ComponentModel.DataAnnotations;
using OcraServer.Enums;

namespace OcraServer.Models.BindingModels
{
    public class ApplicationUserRegistrationTokenBindingModel
    {
        [Required]
        public String DeviceID { get; set; }

        [Required]
        public ClientPlatform DevicePlatform { get; set; }
    }
}

