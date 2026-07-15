import api from './axios';
import type { Task, TaskCreatePayload, TaskFilters, TaskUpdatePayload } from './types';

export const TasksService = {
  // Get a list of tasks for a specific project
  async getTasks(filters: TaskFilters): Promise<Task[]>{
    const params: Record<string, any> = { projectId: filters.projectId };
    if (filters.status !== undefined && filters.status !== '') params.status = filters.status;
    if (filters.status !== undefined && filters.status !== '') params.status = filters.status;
    if (filters.isDescending !== undefined) params.isDescending = filters.isDescending;
    const response = await api.get<Task[]>('/api/Tasks', { params });
    return response.data;
  },

  async update(id: number, payload: TaskUpdatePayload): Promise<void>{
    await api.put(`/api/Tasks/${id}`, payload);
  },

  // Create a new task
  async create(payload: TaskCreatePayload): Promise<void> {
    await api.post('/api/Tasks', payload);
  },

  // Update task status (PATCH request)
  async updateStatus(taskId: number, status: number): Promise<void> {
    await api.patch(`/api/Tasks/${taskId}/status`, status, {
      headers: { 'Content-Type': 'application/json' }
    });
  },

  // Change the person responsible for the task (PATCH request
  async changeAssignee(taskId: number, assigneeId: number): Promise<void> {
    await api.patch(`/api/Tasks/${taskId}/assignee`, assigneeId, {
      headers: { 'Content-Type': 'application/json' }
    });
  },

  // Delete task
  async delete(taskId: number): Promise<void> {
    await api.delete(`/api/Tasks/${taskId}`);
  }
};