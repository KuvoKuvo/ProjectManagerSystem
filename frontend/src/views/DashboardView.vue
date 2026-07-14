<script setup lang="ts">

import { ref, reactive, onMounted, watch } from 'vue'
import { useAuthStore } from '@/stores/auth'
import api from '@/api/axios'

const authStore = useAuthStore()

const isLoading = ref(false)
const errorMessage = ref('')
const projects = ref<any[]>([])

const stats = reactive({
  totalProjects: 0,
  highPriority: 0
})

const filters = reactive({
  startDateFrom: '',
  startDateTo: '',
  priority: '' as number | string,
  sortBy: 'Name',
  isDescending: false
})

const fetchProjects = async() => {
  try{
    isLoading.value = true
    errorMessage.value = ''

    const params: Record<string, any> = {}

    if (filters.startDateFrom) params.startDateFrom = filters.startDateFrom
    if (filters.startDateTo) params.startDateTo = filters.startDateTo
    if (filters.priority) params.priority = Number(filters.priority)

    params.sortBy = filters.sortBy
    params.isDescending = filters.isDescending

    const response = await api.get('/api/Projects', { params })
    projects.value = response.data

    stats.totalProjects = projects.value.length
    stats.highPriority = projects.value.filter(p => p.priority >= 3).length 
  }
  catch (err: any){
    console.error('Failed to load projects:', err)
    errorMessage.value = err.response?.data?.message || 'Failed to load projects list.'
  }
  finally{
    isLoading.value = false
  }
}

const deleteProject = async (id: number, name: string) => {
  if (!confirm(`Are you sure you want to delete project "${name}"? This action cannot be undone.`)) {
    return
  }
  try{
    isLoading.value = true
    await api.delete(`/api/Projects/${id}`)
    await fetchProjects()
  }
  catch (err: any){
    console.error('Delete project error:', err)
    alert(err.response?.data?.message || 'Failed to delete the project. Check your permissions.')
  }
  finally{
    isLoading.value = false
  }
}

const toggleSort = (field: string) => {
  if(filters.sortBy === field){
    filters.isDescending = !filters.isDescending
  }
  else{
    filters.sortBy = field
    filters.isDescending = false
  }
}

const resetFilters = () => {
  filters.startDateFrom = ''
  filters.startDateTo = ''
  filters.priority = ''
  filters.sortBy = 'Name'
  filters.isDescending = false
}

watch([() => filters.startDateFrom, () => filters.startDateTo, () => filters.priority, () => 
filters.sortBy, () => filters.isDescending], () => {
  fetchProjects()
})

onMounted(() => {
  fetchProjects()
})

</script>

<template>
  <div class="min-h-screen bg-slate-50 py-8 px-4 sm:px-6 lg:px-8">
    <div class="max-w-7xl mx-auto space-y-8">
      
      <div class="bg-slate-900 rounded-2xl p-6 md:p-8 text-white shadow-xl flex flex-col md:flex-row justify-between items-start md:items-center gap-6">
        <div>
          <div class="flex items-center gap-3">
            <span class="text-3xl">👋</span>
            <div>
              <h1 class="text-2xl font-bold tracking-tight">Welcome, {{ authStore.user?.email }}</h1>
              <p class="text-slate-400 text-sm mt-0.5">
                Role: <span class="text-emerald-400 font-semibold">{{ authStore.user?.role }}</span>
              </p>
            </div>
          </div>
        </div>
        
        <div class="flex flex-wrap gap-3">
          <router-link 
            v-if="authStore.isDirector || authStore.isProjectManager"
            :to="{ name: 'create-project' }" 
            class="px-4 py-2.5 bg-emerald-600 hover:bg-emerald-500 text-white text-sm font-semibold rounded-xl transition-colors shadow-md flex items-center gap-2 cursor-pointer"
          >
            <span>➕</span> Create Project
          </router-link>

          <router-link 
            v-if="authStore.isDirector"
            :to="{ name: 'create-employee' }" 
            class="px-4 py-2.5 bg-slate-800 hover:bg-slate-700 border border-slate-700 text-white text-sm font-semibold rounded-xl transition-colors flex items-center gap-2 cursor-pointer"
          >
            <span>👤</span> Register Employee
          </router-link>

          <button 
            @click="authStore.logout()" 
            class="px-4 py-2.5 bg-rose-950/40 hover:bg-rose-900/60 text-rose-300 hover:text-rose-200 text-sm font-semibold rounded-xl transition-colors border border-rose-900/50 cursor-pointer"
          >
            Logout
          </button>
        </div>
      </div>

      <div class="grid grid-cols-1 md:grid-cols-2 gap-5">
        <div class="bg-white p-6 rounded-2xl shadow-sm border border-slate-100 flex items-center justify-between">
          <div>
            <p class="text-sm font-medium text-slate-500">Total Available Projects</p>
            <p class="text-3xl font-bold text-slate-900 mt-1">{{ stats.totalProjects }}</p>
          </div>
          <div class="text-3xl p-3 bg-emerald-50 text-emerald-600 rounded-xl">📁</div>
        </div>

        <div class="bg-white p-6 rounded-2xl shadow-sm border border-slate-100 flex items-center justify-between">
          <div>
            <p class="text-sm font-medium text-slate-500">High Priority Projects (3+)</p>
            <p class="text-3xl font-bold text-slate-900 mt-1">{{ stats.highPriority }}</p>
          </div>
          <div class="text-3xl p-3 bg-amber-50 text-amber-600 rounded-xl">🔥</div>
        </div>
      </div>

      <div class="bg-white p-6 rounded-2xl shadow-sm border border-slate-100 space-y-4">
        <div class="flex items-center justify-between border-b border-slate-100 pb-3">
          <h2 class="text-lg font-bold text-slate-800 flex items-center gap-2">
            <span>⚙️</span> Filters & Sorting
          </h2>
          <button 
            @click="resetFilters" 
            class="text-xs text-slate-500 hover:text-slate-900 font-semibold cursor-pointer"
          >
            Reset Filters
          </button>
        </div>

        <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
          <div>
            <label class="block text-xs font-bold text-slate-500 uppercase tracking-wider mb-1">Start Date From</label>
            <input 
              v-model="filters.startDateFrom" 
              type="date" 
              class="w-full rounded-xl border border-slate-200 px-3 py-2 text-sm focus:border-emerald-500 focus:outline-none bg-slate-50"
            />
          </div>

          <div>
            <label class="block text-xs font-bold text-slate-500 uppercase tracking-wider mb-1">Start Date To</label>
            <input 
              v-model="filters.startDateTo" 
              type="date" 
              class="w-full rounded-xl border border-slate-200 px-3 py-2 text-sm focus:border-emerald-500 focus:outline-none bg-slate-50"
            />
          </div>

          <div>
            <label class="block text-xs font-bold text-slate-500 uppercase tracking-wider mb-1">Priority</label>
            <select 
              v-model="filters.priority" 
              class="w-full rounded-xl border border-slate-200 px-3 py-2 text-sm focus:border-emerald-500 focus:outline-none bg-slate-50"
            >
              <option value="">All Priorities</option>
              <option value="1">1 (Low)</option>
              <option value="2">2 (Medium)</option>
              <option value="3">3 (High)</option>
              <option value="4">4 (Critical)</option>
              <option value="5">5 (Highest)</option>
            </select>
          </div>

          <div class="flex items-end">
            <button 
              @click="filters.isDescending = !filters.isDescending"
              class="w-full py-2 px-4 border border-slate-200 hover:bg-slate-50 rounded-xl text-sm font-semibold text-slate-700 flex items-center justify-center gap-2 cursor-pointer transition-colors"
            >
              Direction: {{ filters.isDescending ? 'Descending ⬇️' : 'Ascending ⬆️' }}
            </button>
          </div>
        </div>
      </div>

      <div class="bg-white rounded-2xl shadow-sm border border-slate-100 overflow-hidden">
        <div v-if="isLoading && projects.length === 0" class="p-12 text-center text-slate-500">
          <p class="animate-pulse">Loading projects from server...</p>
        </div>

        <div v-else-if="errorMessage" class="p-8 text-center text-red-600">
          ⚠️ {{ errorMessage }}
        </div>

        <div v-else-if="projects.length === 0" class="p-12 text-center text-slate-500 space-y-2">
          <p class="text-xl">No projects found</p>
          <p class="text-sm text-slate-400">Try changing your filters or create a new project to start.</p>
        </div>

        <div v-else class="overflow-x-auto">
          <table class="w-full border-collapse text-left text-sm">
            <thead class="bg-slate-50 border-b border-slate-100 text-slate-500 uppercase tracking-wider font-bold text-xs">
              <tr>
                <th @click="toggleSort('Name')" class="px-6 py-4 cursor-pointer hover:bg-slate-100 transition-colors">
                  Project Name <span v-if="filters.sortBy === 'Name'">{{ filters.isDescending ? '⬇️' : '⬆️' }}</span>
                </th>
                <th @click="toggleSort('StartDate')" class="px-6 py-4 cursor-pointer hover:bg-slate-100 transition-colors">
                  Dates <span v-if="filters.sortBy === 'StartDate'">{{ filters.isDescending ? '⬇️' : '⬆️' }}</span>
                </th>
                <th class="px-6 py-4">Companies</th>
                <th @click="toggleSort('Priority')" class="px-6 py-4 cursor-pointer hover:bg-slate-100 transition-colors">
                  Priority <span v-if="filters.sortBy === 'Priority'">{{ filters.isDescending ? '⬇️' : '⬆️' }}</span>
                </th>
                <th class="px-6 py-4 text-right">Actions</th>
              </tr>
            </thead>
            <tbody class="divide-y divide-slate-100 font-medium text-slate-700">
              <tr 
                v-for="project in projects" 
                :key="project.id" 
                class="hover:bg-slate-50/50 transition-colors"
              >
                <td class="px-6 py-4">
                  <div class="font-bold text-slate-900 text-base">{{ project.name }}</div>
                  <div class="text-xs text-slate-400 mt-0.5">ID: {{ project.id }}</div>
                </td>
                
                <td class="px-6 py-4">
                  <div class="text-slate-900">Start: {{ new Date(project.startDate).toLocaleDateString() }}</div>
                  <div class="text-slate-400 text-xs">End: {{ project.endDate ? new Date(project.endDate).toLocaleDateString() : 'Not set' }}</div>
                </td>

                <td class="px-6 py-4">
                  <div class="text-xs text-slate-400 uppercase tracking-wide font-bold">Customer</div>
                  <div class="text-slate-900 mb-1">{{ project.customerCompany }}</div>
                  <div class="text-xs text-slate-400 uppercase tracking-wide font-bold">Executor</div>
                  <div class="text-slate-900">{{ project.executorCompany }}</div>
                </td>

                <td class="px-6 py-4">
                  <span 
                    class="inline-flex items-center px-2.5 py-1 rounded-full text-xs font-bold"
                    :class="{
                      'bg-slate-100 text-slate-800': project.priority === 1,
                      'bg-blue-50 text-blue-700': project.priority === 2,
                      'bg-amber-50 text-amber-700': project.priority === 3,
                      'bg-orange-50 text-orange-700': project.priority === 4,
                      'bg-red-50 text-red-700': project.priority >= 5,
                    }"
                  >
                    Level {{ project.priority }}
                  </span>
                </td>

                <td class="px-6 py-4 text-right">
                  <div class="flex items-center justify-end gap-2">
                    <router-link 
                      :to="{ name: 'project-details', params: { id: project.id } }" 
                      class="px-3 py-1.5 bg-slate-100 hover:bg-slate-200 text-slate-700 rounded-lg text-xs font-bold transition-colors cursor-pointer"
                    >
                      Open
                    </router-link>
                    
                    <button 
                      v-if="authStore.isDirector"
                      @click="deleteProject(project.id, project.name)"
                      class="px-3 py-1.5 bg-rose-50 hover:bg-rose-100 text-rose-600 rounded-lg text-xs font-bold transition-colors cursor-pointer"
                    >
                      Delete
                    </button>
                  </div>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>

    </div>
  </div>
</template>