<script setup lang="ts">

import { ref, computed, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import api from '@/api/axios'

const route = useRoute()
const router = useRouter()
const authStore = useAuthStore()

const projectId = Number(route.params.id)

const isLoading = ref(true)
const isUploading = ref(false)
const isSavingTask = ref(false)
const errorMessage = ref('')
const project = ref<any>(null)
const tasks = ref<any[]>([])

const showCreateTaskModal = ref(false)
const fileInput = ref<HTMLInputElement | null>(null)

const newTask = ref({
  name: '',
  comment: '',
  priority: 3,
  status: 0,
  assigneeId: null as number | null
})

const isManagerOrDirector = computed(() => {
  return authStore.isProjectManager || authStore.isDirector
})

const fetchProjectDetails = async () => {
  const response = await api.get(`/api/Projects/${projectId}`)
  project.value = response.data
}

const fetchProjectTasks = async () => {
  const response = await api.get(`/api/tasks`, { params: { projectId } })
  tasks.value = response.data
}

const loadAllPageData = async () => {
  try {
    isLoading.value = true
    errorMessage.value = ''
    
    await Promise.all([
      fetchProjectDetails(),
      fetchProjectTasks()
    ])
  } 
  catch (err: any) {
    console.error('Failed to load page data:', err)
    errorMessage.value = 'Failed to load project data.'
  } 
  finally {
    isLoading.value = false
  }
}

const downloadDocument = async (docId: number, fileName: string) => {
    try{
        const response = await api.get(`/api/projects/${projectId}/documents/${docId}`, {
            responseType: 'blob'
        })

        const url = window.URL.createObjectURL(new Blob([response.data]))
        const link = document.createElement('a')
        link.href = url
        link.setAttribute('download', fileName)
        document.body.appendChild(link)
        link.click()

        link.parentNode?.removeChild(link)
        window.URL.revokeObjectURL(url)
    }
    catch(err){
        console.error('Failed to download document:', err)
        alert('Failed to download document.')
    }
}

const handleFileUpload = async (event: Event) => {
    const target = event.target as HTMLInputElement
    if (!target.files || target.files.length === 0) return

    const file = target.files[0]
    if (!file) return

    try{
        isUploading.value = true
        const formData = new FormData()
        formData.append('file', file)

        await api.post(`/api/projects/${projectId}/documents`, formData, {
            headers: { 'Content-Type': 'multipart/form-data' }
        })

        target.value = ''
        await fetchProjectDetails()

    }
    catch(err: any){
        console.error('Failed to upload document:', err)
        alert(err.response?.data?.message || 'Failed to upload document.')
    }
    finally{
        isUploading.value = false
    }
}

const handleCreateTask = async () => {
  if (!newTask.value.name.trim() || !newTask.value.assigneeId) {
    alert('Please fill out the task name and select an assignee.')
    return
  }
  try{
    isSavingTask.value = true
    const payload = {
      name: newTask.value.name,
      comment: newTask.value.comment,
      priority: Number(newTask.value.priority),
      status: Number(newTask.value.status),
      projectId: projectId,
      authorId: authStore.user?.id || 1,
      assigneeId: newTask.value.assigneeId
    }

    await api.post('/api/tasks', payload)
    showCreateTaskModal.value = false

    newTask.value = {
      name: '',
      comment: '',
      priority: 3,
      status: 0,
      assigneeId: null
    }

    await fetchProjectTasks()
  }
  catch(err: any){
    console.error('Failed to create task:', err)
    alert(err.response?.data?.message || 'Failed to create task.')
  }
  finally{
    isSavingTask.value = false
  }
}

const updateTaskStatus = async (taskId: number, newStatus: number) => {
  try{
    await api.patch(`/api/tasks/${taskId}/status`, newStatus, {
      headers: { 'Content-Type': 'application/json' }
    })
    await fetchProjectTasks()
  }
  catch (err: any){
    console.error('Failed to update status:', err)
    alert(err.response?.data?.message || 'Failed to update task status.')
  }
}

const changeTaskAssignee = async (taskId: number, newAssigneeId: number) => {
  try{
    await api.patch(`/api/tasks/${taskId}/assignee`, newAssigneeId, {
      headers: { 'Content-Type': 'application/json' }
    })
    await fetchProjectTasks()
  }
  catch(err: any){
    console.error('Failed to change assignee:', err)
    alert(err.response?.data?.message || 'Failed to change assignee.')
  }
}

const deleteTask = async (taskId: number) => {
  if (!confirm('Are you sure you want to delete this task?')) return
  try{
    await api.delete(`/api/tasks/${taskId}`)
    await fetchProjectTasks()
  }
  catch(err: any){
    console.error('Failed to delete task:', err)
    alert(err.response?.data?.message || 'Failed to delete task.')
  }
}

const getPriorityLabel = (priority: number) => {
  if (priority <= 2) return { text: 'Low', class: 'bg-slate-100 text-slate-800' }
  if (priority === 3) return { text: 'Medium', class: 'bg-blue-50 text-blue-700' }
  if (priority === 4) return { text: 'High', class: 'bg-orange-50 text-orange-700' }
  return { text: 'Critical', class: 'bg-red-50 text-red-700' }
}

const getStatusDetails = (status: number) => {
  switch (status) {
    case 0: return { text: 'To Do', class: 'bg-slate-100 text-slate-600 border-slate-200' }
    case 1: return { text: 'In Progress', class: 'bg-indigo-50 text-indigo-600 border-indigo-100' }
    case 2: return { text: 'Done', class: 'bg-emerald-50 text-emerald-600 border-emerald-100' }
    default: return { text: 'Unknown', class: 'bg-slate-100 text-slate-600' }
  }
}

const projectEmployeesList = computed(() => {
  return project.value?.assignedEmployees || []
})

const projectDocumentsList = computed(() => {
  return project.value?.documents || []
})

onMounted(() => {
  loadAllPageData()
})

</script>

<template>
  <div class="min-h-screen bg-slate-50 py-8 px-4 sm:px-6 lg:px-8">
    <div class="max-w-7xl mx-auto space-y-6">
      
      <div class="flex justify-between items-center">
        <button 
          @click="router.push({ name: 'dashboard' })" 
          class="inline-flex items-center gap-2 text-sm font-semibold text-slate-600 hover:text-slate-900 transition-colors cursor-pointer"
        >
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
          
          <div class="bg-white rounded-2xl p-6 md:p-8 shadow-sm border border-slate-100 space-y-6">
            <div class="flex flex-col md:flex-row md:items-center justify-between gap-4 border-b border-slate-100 pb-5">
              <div>
                <span 
                  class="inline-flex items-center px-2.5 py-1 rounded-full text-xs font-bold mb-2"
                  :class="{
                    'bg-slate-100 text-slate-800': project.priority === 1,
                    'bg-blue-50 text-blue-700': project.priority === 2,
                    'bg-amber-50 text-amber-700': project.priority === 3,
                    'bg-orange-50 text-orange-700': project.priority === 4,
                    'bg-red-50 text-red-700': project.priority >= 5,
                  }"
                >
                  Priority Level {{ project.priority }}
                </span>
                <h1 class="text-3xl font-extrabold text-slate-900 tracking-tight">{{ project.name }}</h1>
              </div>
            </div>

            <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
              <div class="space-y-4">
                <h3 class="text-sm font-bold text-slate-400 uppercase tracking-wider">Counterparties</h3>
                <div class="bg-slate-50 rounded-xl p-4 space-y-3">
                  <div>
                    <p class="text-xs text-slate-500">Customer Company</p>
                    <p class="font-bold text-slate-800">{{ project.customerCompany }}</p>
                  </div>
                  <div class="border-t border-slate-200/60 pt-2">
                    <p class="text-xs text-slate-500">Executing Company</p>
                    <p class="font-bold text-slate-800">{{ project.executorCompany }}</p>
                  </div>
                </div>
              </div>

              <div class="space-y-4">
                <h3 class="text-sm font-bold text-slate-400 uppercase tracking-wider">Project Timeline</h3>
                <div class="bg-slate-50 rounded-xl p-4 space-y-3">
                  <div>
                    <p class="text-xs text-slate-500">Start Date</p>
                    <p class="font-bold text-slate-800">{{ new Date(project.startDate).toLocaleDateString() }}</p>
                  </div>
                  <div class="border-t border-slate-200/60 pt-2">
                    <p class="text-xs text-slate-500">End Date</p>
                    <p class="font-bold text-slate-800">
                      {{ project.endDate ? new Date(project.endDate).toLocaleDateString() : 'Infinite / Not Set' }}
                    </p>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <div class="bg-white rounded-2xl p-6 md:p-8 shadow-sm border border-slate-100 space-y-6">
            <div class="flex items-center justify-between border-b border-slate-100 pb-4">
              <div>
                <h2 class="text-xl font-bold text-slate-900">Tasks</h2>
                <p class="text-xs text-slate-400 mt-0.5">Manage workflow and track assignees</p>
              </div>
              <button 
                v-if="isManagerOrDirector"
                @click="showCreateTaskModal = true"
                class="px-4 py-2 bg-slate-900 hover:bg-slate-800 text-white text-xs font-bold rounded-xl transition-colors cursor-pointer flex items-center gap-1.5"
              >
                <span>➕ Create Task</span>
              </button>
            </div>

            <div v-if="tasks.length > 0" class="space-y-4">
              <div 
                v-for="task in tasks" 
                :key="task.id" 
                class="p-5 bg-white border border-slate-200/80 rounded-2xl shadow-sm hover:shadow-md transition-all space-y-4"
              >
                <div class="flex items-start justify-between gap-4">
                  <div>
                    <div class="flex items-center gap-2 flex-wrap">
                      <span class="inline-flex px-2 py-0.5 rounded text-xs font-semibold" :class="getPriorityLabel(task.priority).class">
                        {{ getPriorityLabel(task.priority).text }}
                      </span>
                      <span class="inline-flex px-2 py-0.5 rounded text-xs font-semibold border" :class="getStatusDetails(task.status).class">
                        {{ getStatusDetails(task.status).text }}
                      </span>
                    </div>
                    <h4 class="text-base font-bold text-slate-800 mt-2">{{ task.name }}</h4>
                    <p class="text-sm text-slate-500 mt-1" v-if="task.comment">{{ task.comment }}</p>
                  </div>

                  <button 
                    v-if="isManagerOrDirector"
                    @click="deleteTask(task.id)"
                    class="text-xs text-red-500 hover:text-red-700 font-bold p-1 hover:bg-red-50 rounded transition-all cursor-pointer"
                    title="Delete task"
                  >
                    🗑️
                  </button>
                </div>

                <div class="border-t border-slate-100 pt-4 flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4 text-xs">
                  
                  <div class="flex items-center gap-2">
                    <span class="text-slate-400">Assignee:</span>
                    <div v-if="isManagerOrDirector">
                      <select 
                        :value="task.assigneeId" 
                        @change="changeTaskAssignee(task.id, Number(($event.target as HTMLSelectElement).value))"
                        class="bg-slate-50 border border-slate-200 text-slate-700 text-xs rounded-lg p-1 font-semibold focus:ring-1 focus:ring-slate-300 outline-none"
                      >
                        <option v-for="emp in projectEmployeesList" :key="emp.id" :value="emp.id">
                          {{ emp.fullName }}
                        </option>
                      </select>
                    </div>
                    <span v-else class="font-bold text-slate-700 bg-slate-100 py-0.5 px-2 rounded-full">
                      {{ task.assigneeFullName }}
                    </span>
                  </div>

                  <div class="flex items-center gap-2">
                    <span class="text-slate-400">Change Status:</span>
                    <div class="flex gap-1">
                      <button 
                        @click="updateTaskStatus(task.id, 0)" 
                        :disabled="task.status === 0"
                        class="px-2 py-1 border rounded hover:bg-slate-50 disabled:bg-slate-100 disabled:text-slate-400 disabled:cursor-not-allowed font-semibold"
                      >
                        To Do
                      </button>
                      <button 
                        @click="updateTaskStatus(task.id, 1)" 
                        :disabled="task.status === 1"
                        class="px-2 py-1 border rounded hover:bg-slate-50 disabled:bg-indigo-500 disabled:text-white disabled:border-indigo-500 disabled:cursor-not-allowed font-semibold"
                      >
                        In Work
                      </button>
                      <button 
                        @click="updateTaskStatus(task.id, 2)" 
                        :disabled="task.status === 2"
                        class="px-2 py-1 border rounded hover:bg-slate-50 disabled:bg-emerald-500 disabled:text-white disabled:border-emerald-500 disabled:cursor-not-allowed font-semibold"
                      >
                        Done
                      </button>
                    </div>
                  </div>

                </div>
              </div>
            </div>

            <div v-else class="text-center py-10 bg-slate-50 rounded-xl border border-dashed border-slate-200">
              <p class="text-sm text-slate-400">No tasks created yet for this project.</p>
            </div>
          </div>

          <div class="bg-white rounded-2xl p-6 md:p-8 shadow-sm border border-slate-100 space-y-6">
            <div class="flex items-center justify-between border-b border-slate-100 pb-4">
              <div>
                <h2 class="text-xl font-bold text-slate-900">Project Documents</h2>
                <p class="text-xs text-slate-400 mt-0.5">Manage and download technical attachments</p>
              </div>
              
              <div>
                <input 
                  type="file" 
                  ref="fileInput" 
                  @change="handleFileUpload" 
                  class="hidden" 
                />
                <button 
                  @click="fileInput?.click()" 
                  :disabled="isUploading"
                  class="px-4 py-2 bg-slate-900 hover:bg-slate-800 text-white text-xs font-bold rounded-xl transition-colors cursor-pointer flex items-center gap-1.5 disabled:opacity-50"
                >
                  <span>{{ isUploading ? 'Uploading...' : '📁 Add File' }}</span>
                </button>
              </div>
            </div>

            <div v-if="projectDocumentsList.length > 0" class="divide-y divide-slate-100 border border-slate-100 rounded-xl overflow-hidden bg-white">
              <div 
                v-for="doc in projectDocumentsList" 
                :key="doc.id" 
                class="px-4 py-3.5 flex justify-between items-center hover:bg-slate-50/50 transition-all"
              >
                <div class="flex items-center gap-3 min-w-0">
                  <span class="text-2xl">📄</span>
                  <div class="min-w-0">
                    <p class="text-sm font-bold text-slate-800 truncate max-w-xs md:max-w-md">{{ doc.fileName }}</p>
                    <p class="text-xs text-slate-400 font-mono mt-0.5">Attachment</p>
                  </div>
                </div>
                <button 
                  @click="downloadDocument(doc.id, doc.fileName)"
                  class="text-xs text-emerald-600 hover:text-emerald-500 font-bold hover:underline cursor-pointer flex items-center gap-1"
                >
                  Download
                </button>
              </div>
            </div>

            <div v-else class="text-center py-8 bg-slate-50 rounded-xl border border-dashed border-slate-200">
              <p class="text-sm text-slate-400">No documents uploaded yet for this project.</p>
            </div>
          </div>

        </div>

        <div class="space-y-6">
          <div class="bg-white rounded-2xl p-6 shadow-sm border border-slate-100 space-y-6">
            <h2 class="text-xl font-bold text-slate-900 border-b border-slate-100 pb-4">Project Team</h2>

            <div class="space-y-3">
              <h3 class="text-xs font-bold text-slate-400 uppercase tracking-wider">Project Manager</h3>
              <div v-if="project.projectManager" class="flex items-center gap-3 p-3 bg-slate-50 rounded-xl border border-slate-100">
                <span class="text-2xl">👑</span>
                <div>
                  <p class="text-sm font-bold text-slate-800">{{ project.projectManager.fullName }}</p>
                  <p class="text-xs text-slate-400">{{ project.projectManager.email }}</p>
                </div>
              </div>
              <div v-else class="text-sm text-amber-600 bg-amber-50 p-3 rounded-xl border border-amber-200">
                ⚠️ No manager is assigned to the project!
              </div>
            </div>

            <div class="space-y-3">
              <h3 class="text-xs font-bold text-slate-400 uppercase tracking-wider">Executors ({{ projectEmployeesList.length }})</h3>
              <div v-if="projectEmployeesList.length > 0" class="space-y-2">
                <div 
                  v-for="emp in projectEmployeesList" 
                  :key="emp.id" 
                  class="flex items-center gap-3 p-3 hover:bg-slate-50 rounded-xl border border-transparent hover:border-slate-100 transition-all"
                >
                  <span class="text-xl">👤</span>
                  <div>
                    <p class="text-sm font-bold text-slate-800">{{ emp.fullName }}</p>
                    <p class="text-xs text-slate-400">{{ emp.email }}</p>
                  </div>
                </div>
              </div>
              <div v-else class="text-sm text-slate-400 text-center py-6 border-2 border-dashed border-slate-100 rounded-xl">
                No employees are assigned to this project yet.
              </div>
            </div>
          </div>
        </div>

      </div>

    </div>
  </div>

  <div v-if="showCreateTaskModal" class="fixed inset-0 bg-slate-900/40 backdrop-blur-sm flex items-center justify-center p-4 z-50">
    <div class="bg-white rounded-2xl max-w-md w-full p-6 shadow-xl space-y-4">
      <h3 class="text-lg font-bold text-slate-900">Create New Task</h3>
      
      <div class="space-y-3">
        <div>
          <label class="block text-xs font-bold text-slate-500 uppercase mb-1">Task Title</label>
          <input 
            v-model="newTask.name" 
            type="text" 
            class="w-full bg-slate-50 border border-slate-200 rounded-xl p-2.5 text-sm outline-none focus:border-slate-400"
          />
        </div>

        <div>
          <label class="block text-xs font-bold text-slate-500 uppercase mb-1">Comment / Description</label>
          <textarea 
            v-model="newTask.comment" 
            rows="3"
            class="w-full bg-slate-50 border border-slate-200 rounded-xl p-2.5 text-sm outline-none focus:border-slate-400 resize-none"
          ></textarea>
        </div>

        <div class="grid grid-cols-2 gap-4">
          <div>
            <label class="block text-xs font-bold text-slate-500 uppercase mb-1">Priority</label>
            <select 
              v-model="newTask.priority"
              class="w-full bg-slate-50 border border-slate-200 rounded-xl p-2.5 text-sm outline-none focus:border-slate-400"
            >
              <option :value="2">Low</option>
              <option :value="3">Medium</option>
              <option :value="4">High</option>
              <option :value="5">Critical</option>
            </select>
          </div>

          <div>
            <label class="block text-xs font-bold text-slate-500 uppercase mb-1">Assignee</label>
            <select 
              v-model="newTask.assigneeId"
              class="w-full bg-slate-50 border border-slate-200 rounded-xl p-2.5 text-sm outline-none focus:border-slate-400"
            >
              <option :value="null" disabled>Select Executor</option>
              <option v-for="emp in projectEmployeesList" :key="emp.id" :value="emp.id">
                {{ emp.fullName }}
              </option>
            </select>
          </div>
        </div>
      </div>

      <div class="flex justify-end gap-3 pt-2">
        <button 
          @click="showCreateTaskModal = false"
          class="px-4 py-2 bg-slate-100 hover:bg-slate-200 text-slate-700 text-xs font-bold rounded-xl cursor-pointer"
        >
          Cancel
        </button>
        <button 
          @click="handleCreateTask"
          :disabled="isSavingTask"
          class="px-4 py-2 bg-slate-900 hover:bg-slate-800 text-white text-xs font-bold rounded-xl cursor-pointer disabled:opacity-50"
        >
          {{ isSavingTask ? 'Saving...' : 'Create' }}
        </button>
      </div>
    </div>
  </div>
</template>