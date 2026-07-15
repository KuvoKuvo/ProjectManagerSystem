<script setup lang="ts">
import { useProjectDetails } from '@/composables/useProjectDetails'
import ProjectHeaderInfo from '@/components/projects/ProjectHeaderInfo.vue'
import ProjectTasks from '@/components/projects/ProjectTasks.vue'
import ProjectDocuments from '@/components/projects/ProjectDocuments.vue'
import ProjectTeam from '@/components/projects/ProjectTeam.vue'
import CreateTaskModal from '@/components/projects/modals/CreateTaskModal.vue'
import EditProjectModal from '@/components/projects/modals/EditProjectModal.vue'
import EditTaskModal from '@/components/projects/modals/EditTaskModal.vue'

const {
  router, authStore, isLoading, isUploading, isSavingTask, isSavingProject, errorMessage,
  project, tasks, allManagers, allEmployees, showCreateTaskModal, showEditProjectModal,
  fileInput, editForm, newTask, isManagerOrDirector, projectEmployeesList, projectDocumentsList,
  addEmployeeToProject, removeEmployeeFromProject, openEditModal, handleUpdateProject,
  downloadDocument, handleFileUpload, handleCreateTask, updateTaskStatus, changeTaskAssignee,
  deleteTask, getPriorityLabel, getStatusDetails,
  taskFilters,
  showEditTaskModal,
  editTaskForm,
  openEditTaskModal,
  handleUpdateTask
} = useProjectDetails()
</script>

<template>
  <div class="min-h-screen bg-slate-50 py-8 px-4 sm:px-6 lg:px-8">
    <div class="max-w-7xl mx-auto space-y-6">
      
      <div class="flex justify-between items-center">
        <button @click="router.push({ name: 'dashboard' })" class="inline-flex items-center gap-2 text-sm font-semibold text-slate-600 hover:text-slate-900 transition-colors cursor-pointer">
          ← Back to Dashboard
        </button>
      </div>

      <div v-if="isLoading" class="bg-white rounded-2xl p-12 text-center border border-slate-100 shadow-sm">
        <p class="text-slate-500 animate-pulse text-lg">Loading project details...</p>
      </div>
      <div v-else-if="errorMessage" class="bg-red-50 border border-red-200 text-red-700 rounded-2xl p-8 text-center shadow-sm">
        <p class="text-lg font-semibold">⚠️ Error Loading Project</p>
        <p class="text-sm mt-1">{{ errorMessage }}</p>
      </div>

      <div v-else class="grid grid-cols-1 lg:grid-cols-3 gap-6">
        
        <div class="lg:col-span-2 space-y-6">
          <ProjectHeaderInfo 
            :project="project" 
            :is-manager-or-director="isManagerOrDirector"
            @edit="openEditModal" 
          />
          
          <ProjectTasks 
            :tasks="tasks" 
            :is-manager-or-director="isManagerOrDirector"
            :project-employees-list="projectEmployeesList"
            :get-priority-label="getPriorityLabel"
            :get-status-details="getStatusDetails"
            v-model:filters="taskFilters"
            @edit="openEditTaskModal"
            @create="showCreateTaskModal = true"
            @delete="deleteTask"
            @change-assignee="changeTaskAssignee"
            @update-status="updateTaskStatus"
          />

          <ProjectDocuments 
            :documents="projectDocumentsList"
            :is-uploading="isUploading"
            :is-manager-or-director="isManagerOrDirector"
            @upload="handleFileUpload"
            @download="downloadDocument"
          />
        </div>

        <div class="space-y-6">
          <ProjectTeam 
            :project-manager="project.projectManager"
            :employees="projectEmployeesList"
          />
        </div>
      </div>
    </div>
  </div>

  <CreateTaskModal 
    v-if="showCreateTaskModal"
    v-model:new-task="newTask"
    :is-saving="isSavingTask"
    :employees="projectEmployeesList"
    @close="showCreateTaskModal = false"
    @create="handleCreateTask"
  />

  <EditTaskModal 
    v-if="showEditTaskModal"
    v-model:editTaskForm="editTaskForm"
    :is-saving="isSavingTask"
    @close="showEditTaskModal = false"
    @save="handleUpdateTask"
  />

  <EditProjectModal 
    v-if="showEditProjectModal"
    v-model:edit-form="editForm"
    :is-saving="isSavingProject"
    :project="project"
    :all-managers="allManagers"
    :all-employees="allEmployees"
    :is-director="authStore.isDirector"
    @close="showEditProjectModal = false"
    @save="handleUpdateProject"
    @add-employee="addEmployeeToProject"
    @remove-employee="removeEmployeeFromProject"
  />
</template>