using Coders_Back.Domain.DataAbstractions;
using Coders_Back.Domain.DTOs.Output;
using Coders_Back.Domain.Entities;
using Coders_Back.Domain.Interfaces;

namespace Coders_Back.Domain.Services;

public class ProjectService : IProjectService
{   
    private readonly IRepository<Project> _projects;
    private readonly IRepository<Collaborator> _collaborators;
    
    public ProjectService(IRepository<Project> projects, IRepository<Collaborator> collaborators)
    {
        _projects = projects;
        _collaborators = collaborators;
    }

    public async Task<List<ProjectOutput>> GetAll()
    {
        var projects = await _projects.GetAll();
        return projects.Select(p => new ProjectOutput(p)).ToList();
    }

    public async Task<ProjectOutput?> GetById(Guid projectId)
    {
        var project = await _projects.GetById(projectId);
        return project is null ? null : new ProjectOutput(project);
    }

    public async Task<List<CollaboratorOutput>> GetCollaboratorsByProject(Guid projectId)
    {
        var project = await _projects.GetById(projectId);

        if (project is null)
        {
            return null;
        }

        var collaborators = await _collaborators.GetAll();
        var projectCollaborators = collaborators
            .Where(c => c.ProjectId == projectId)
            .Select(c => new CollaboratorOutput(c))
            .ToList();

        return projectCollaborators;
    }

    public async Task DeleteCollaborator(Guid collaboratorId, Guid userId)
    {
        var collaborator = await _collaborators.GetById(collaboratorId);
        if (collaborator != null) return;
        
        var project = await _projects.GetById(collaborator!.ProjectId);
        if (project != null) return;

        if (project!.OwnerId == userId)
        {
            _collaborators.Delete(collaborator);
        }
    }
}