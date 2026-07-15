import { ref, computed, onMounted, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import { TasksService } from '@/api/tasks.service'
import type { TaskFilters, TaskUpdatePayload } from '@/api/types'
import api from '@/api/axios'

export function useProjectDetails() {
  const route = useRoute()
  const router = useRouter()
  const authStore = useAuthStore()

  const projectId = Number(route.params.id)

  const isLoading = ref(true)
  const isUploading = ref(false)
  const isSavingTask = ref(false)
  const isSavingProject = ref(false)
  const errorMessage = ref('')
  const project = ref<any>(null)
  const tasks = ref<any[]>([])
  const allManagers = ref<Array<{ id: number, fullName: string }>>([])
  const allEmployees = ref<Array<{ id: number, fullName: string, email: string }>>([])

  const showCreateTaskModal = ref(false)
  const showEditProjectModal = ref(false)
  const fileInput = ref<HTMLInputElement | null>(null)

  const editForm = ref({
    id: 0,
    name: '',
    customerCompany: '',
    executorCompany: '',
    startDate: '',
    endDate: '',
    priority: 1,
    projectManagerId: 0,
    employeeIds: [] as number[]
  })

  const newTask = ref({
    name: '',
    comment: '',
    priority: 1,
    status: 0,
    assigneeId: null as number | null
  })

  const taskFilters = ref<TaskFilters>({
    projectId: projectId,
    status: '',
    sortBy: 'priority',
    isDescending: false
  })

  const showEditTaskModal = ref(false)
  const editTaskForm = ref<TaskUpdatePayload>({
    id: 0,
    name: '',
    comment: '',
    priority: 1,
    status: 0,
    projectId: 0,
    assigneeId: 0
  })

  const openEditTaskModal = (task: any) => {
    editTaskForm.value = {
      id: task.id,
      name: task.name,
      comment: task.comment || '',
      priority: task.priority,
      status: task.status,
      projectId: task.projectId,
      assigneeId: task.assigneeId 
    }
    showEditTaskModal.value = true
  }

  const handleUpdateTask = async () => {
    isSavingTask.value = true
    try {
      await TasksService.update(editTaskForm.value.id, editTaskForm.value)
      showEditTaskModal.value = false
      await fetchTasks()
    } catch (e) {
      console.error('Failed to update task:', e)
    } finally {
      isSavingTask.value = false
    }
  }

  const fetchTasks = async () => {
    try {
      tasks.value = await TasksService.getTasks(taskFilters.value)
    } catch (e) {
      console.error('Failed to fetch tasks:', e)
    }
  }

  watch(taskFilters, () => {
    fetchTasks()
  }, { deep: true })

  const isManagerOrDirector = computed(() => {
    if (authStore.isDirector) return true
    
    if (authStore.isProjectManager) {
      return project.value?.projectManagerId === authStore.user?.id
    }
    
    return false
  })

  const projectEmployeesList = computed(() => {
    return project.value?.assignedEmployees || []
  })

  const projectDocumentsList = computed(() => {
    return project.value?.documents || []
  })

  const fetchProjectDetails = async () => {
    const response = await api.get(`/api/Projects/${projectId}`)
    project.value = response.data
  }

  const fetchProjectTasks = async () => {
    const response = await api.get(`/api/tasks`, { params: { projectId } })
    tasks.value = response.data
  }

  const fetchManagers = async () => {
    if (authStore.isDirector){
      try{
        const response = await api.get('/api/employees/managers')
        allManagers.value = response.data
      }
      catch (err){
        console.error('Failed to load managers list:', err)
      }
    }
  }

  const fetchEmployees = async () => {
    try {
      const response = await api.get('/api/employees') 
      allEmployees.value = response.data
    }
    catch (err) {
      console.error('Failed to load employees list:', err)
    }
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

  const addEmployeeToProject = (empId: number) => {
    if (!empId) return
    if (!editForm.value.employeeIds.includes(empId)) {
      editForm.value.employeeIds.push(empId)
    }
  }

  const removeEmployeeFromProject = (empId: number) => {
    editForm.value.employeeIds = editForm.value.employeeIds.filter(id => id !== empId)
  }

  const openEditModal = async () => {
    if (!project.value) return

    if (authStore.isDirector) {
      await fetchManagers()
    }
    await fetchEmployees()
    
    editForm.value = {
      id: project.value.id,
      name: project.value.name,
      customerCompany: project.value.customerCompany,
      executorCompany: project.value.executorCompany,
      startDate: project.value.startDate ? project.value.startDate.split('T')[0] : '',
      endDate: project.value.endDate ? project.value.endDate.split('T')[0] : '',
      priority: project.value.priority,
      projectManagerId: project.value.projectManagerId,
      employeeIds: project.value.assignedEmployees?.map((e: any) => e.id) || []
    }
    showEditProjectModal.value = true
  }

  const handleUpdateProject = async () => {
    if (!editForm.value.name.trim() || !editForm.value.customerCompany.trim() || !editForm.value.executorCompany.trim() || !editForm.value.startDate) {
      alert('Please fill out all required fields.')
      return
    }
    try{
      isSavingProject.value = true
      const payload = {
        ...editForm.value,
      }
      await api.put(`/api/projects/${projectId}`, payload)
      showEditProjectModal.value = false
      await fetchProjectDetails()
    }
    catch(err: any){
      console.error('Failed to update project:', err)
      alert(err.response?.data?.message || 'Failed to update baseline project information.')
    }
    finally{
      isSavingProject.value = false
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

  onMounted(() => {
    loadAllPageData()
  })

  return {
    router,
    authStore,
    projectId,
    isLoading,
    isUploading,
    isSavingTask,
    isSavingProject,
    errorMessage,
    project,
    tasks,
    allManagers,
    allEmployees,
    showCreateTaskModal,
    showEditProjectModal,
    fileInput,
    editForm,
    newTask,
    isManagerOrDirector,
    projectEmployeesList,
    projectDocumentsList,
    addEmployeeToProject,
    removeEmployeeFromProject,
    openEditModal,
    handleUpdateProject,
    downloadDocument,
    handleFileUpload,
    handleCreateTask,
    updateTaskStatus,
    changeTaskAssignee,
    deleteTask,
    getPriorityLabel,
    getStatusDetails,
    
    taskFilters,
    showEditTaskModal,
    editTaskForm,
    openEditTaskModal,
    handleUpdateTask
  }
}