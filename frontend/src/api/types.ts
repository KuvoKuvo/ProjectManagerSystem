export type UserRole = 'Employee' | 'ProjectManager' | 'Director';

export interface Employee {
    id: number;
    firstName: string;
    lastName: string;
    middleName?: string;
    email: string;
    role: UserRole;
}

export interface EmployeeCreatePayload {
    firstName: string;
    lastName: string;
    middleName?: string;
    email: string;
    role: UserRole;
}

export interface Project {
    id: number;
    name: string;
    startDate: string;
    endDate?: string | null;
    priority: number;
    customerCompany: string;
    executorCompany: string;
    projectManagerId: number | null;
    projectManagerName?: string;
    assignedEmployees?: Employee[];
    documents?: ProjectDocument[];
}

export interface ProjectCreatePayload {
    name: string;
    startDate: string;
    endDate?: string | null;
    priority: number;
    customerCompany: string;
    executorCompany: string;
    projectManagerId: number | null;
    employeeIds: number[];
}

export interface ProjectDocument {
    id: number;
    fileName: string;
}

export interface Task {
    id: number;
    name: string;
    comment?: string;
    priority: number;
    status: number;
    projectId: number;
    authorId: number;
    assigneeId: number;
}

export interface TaskCreatePayload {
    name: string;
    comment?: string;
    priority: number;
    status: number;
    projectId: number;
    authorId: number;
    assigneeId: number;
}

export interface ProjectFilters {
    startDateFrom?: string;
    startDateTo?: string;
    priority?: number;
    sortBy?: string;
    isDescending?: boolean;
}

export interface ChangePasswordPayload {
  currPassword: string;
  newPassword: string;
}

export interface TaskFilters {
    projectId: number;
    status?: number | '';
    sortBy?: string;
    isDescending?: boolean;
}

export interface TaskUpdatePayload {
    id: number;
    name: string;
    comment?: string;
    priority: number;
    status: number;
    projectId: number;
    assigneeId: number;
}