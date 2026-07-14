import api from './axios';
import type { Task, TaskCreatePayload } from './types';

export const TasksService = {
  // Get a list of tasks for a specific project
  async getByProjectId(projectId: number): Promise<Task[]> {
    const response = await api.get<Task[]>('/api/Tasks', {
      params: { projectId }
    });
    return response.data;
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