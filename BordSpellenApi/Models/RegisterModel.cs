﻿using System.ComponentModel.DataAnnotations;
using Core.Domain.Attributes;
using Core.Domain.Dto.Actors;
using Core.Domain.Enums;

namespace BordSpellenApi.Models;

public class RegisterModel
{
    [Display(Name = "First Name")]
    [Required(ErrorMessage = "First name is required")]
    public string? FirstName { get; set; }
    
    [Display(Name = "Last Name")]
    [Required(ErrorMessage = "First name is required")]
    public string? LastName { get; set; }

    [Display(Name = "Email")]
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress]
    public string? Email { get; set; }
    
    [Display(Name="Gender")]
    [Required(ErrorMessage = "Gender is required")]
    public GenderEnum Gender { get; set; }
    
    [Display(Name="Date of Birth")]
    [Required(ErrorMessage = "Date of Birth is required")] 
    [DataType(DataType.Date)]
    [DatePast(ErrorMessage = "Date cannot exceed today's date")]
    public DateTime? DateOfBirth { get; set; }
    
    [Display(Name="Street")]
    [Required(ErrorMessage = "Street is required")]
    public string? Street { get; set; }
    
    [Display(Name="House number")]
    [Required(ErrorMessage = "House number is required")]
    public string? HouseNumber { get; set; }
    
    [Display(Name="Postal code")]
    [Required(ErrorMessage = "Postal code is required")]
    public string? PostalCode { get; set; }
    
    [Display(Name="City")]
    [Required(ErrorMessage = "City is required")]
    public string? City { get; set; }
    
    [Display(Name="Phone number")]
    [Required (ErrorMessage = "Phone number is required")]
    [Phone]
    public string? PhoneNumber { get; set; }

    [Display(Name = "Diet preferences")]
    public List<int>? DietPreferences { get; set; }

    [Required(ErrorMessage = "Password is required")]
    [DataType(DataType.Password)]
    public string Password { get; set; }
        
    [Display(Name="Confirm password")]
    [Required(ErrorMessage = "Confirm password is required")]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Passwords do not match")]
    public string ConfirmPassword { get; set; }

    public PersonDto ToDto(RegisterModel model)
    {
        return new PersonDto
        {
            FirstName = model.FirstName,
            LastName = model.LastName,
            Email = model.Email,
            Gender = model.Gender,
            DateOfBirth = model.DateOfBirth,
            Street = model.Street,
            HouseNumber = model.HouseNumber,
            PostalCode = model.PostalCode,
            City = model.City,
            PhoneNumber = model.PhoneNumber,
            DietPreferences = model.DietPreferences
        };
    }
}