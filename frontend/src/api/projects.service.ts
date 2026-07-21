import api from './axios';
import type { Project, ProjectCreatePayload, ProjectFilters, PagedResult } from './types';

export const ProjectsService = {
  // Get a filtered and sorted list of projects
  async getAll(filters: ProjectFilters = {}): Promise<PagedResult<Project>> {
    const params: Record<string, any> = {};

    if (filters.startDateFrom) params.startDateFrom = filters.startDateFrom;
    if (filters.startDateTo) params.startDateTo = filters.startDateTo;
    if (filters.priority) params.priority = Number(filters.priority);
    
    params.sortBy = filters.sortBy || 'Name';
    params.isDescending = filters.isDescending ?? false;

    params.pageNumber = filters.pageNumber || 1;
    params.pageSize = filters.pageSize || 10;

    const response = await api.get<PagedResult<Project>>('/api/Projects', { params });
    return response.data;
  },

  // Get detailed project information by Id
  async getById(id: number): Promise<Project> {
    const response = await api.get<Project>(`/api/Projects/${id}`);
    return response.data;
  },

  // Create a basic project
  async create(payload: ProjectCreatePayload): Promise<{ id: number }> {
    const response = await api.post<{ id: number }>('/api/Projects', payload);
    return response.data;
  },

  // Update basic project information
  async update(id: number, payload: Partial<ProjectCreatePayload> & { id: number }): Promise<void> {
    await api.put(`/api/Projects/${id}`, payload);
  },

  // Delete project by Id
  async delete(id: number): Promise<void> {
    await api.delete(`/api/Projects/${id}`);
  },

  // Upload a file to the project
  async uploadDocument(projectId: number, file: File): Promise<void> {
    const formData = new FormData();
    formData.append('file', file);
    await api.post(`/api/Projects/${projectId}/documents`, formData, {
      headers: { 'Content-Type': 'multipart/form-data' }
    });
  },

  // Download project document by document I
  async downloadDocument(projectId: number, documentId: number): Promise<Blob> {
    const response = await api.get(`/api/Projects/${projectId}/documents/${documentId}`, {
      responseType: 'blob'
    });
    return response.data;
  }
};