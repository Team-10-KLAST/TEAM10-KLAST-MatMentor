using System;
using System.Collections.Generic;
using System.Text;
using Application.Features.Students.CreateStudentProfile;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Students;

public class StudentRepository : ICreateStudentProfileRepository
{
    private readonly AppDbContext _context;

    public StudentRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Student student)
    {
        _context.Students.Add(student);
        await _context.SaveChangesAsync();
    }

    public async Task<Interest?> GetInterestByIdAsync(int interestId)
    {
        return await _context.Interests.FirstOrDefaultAsync(i => i.Id == interestId);
    }
}
