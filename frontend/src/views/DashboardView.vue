<script setup lang="ts">

import { useDashboard } from '@/composables/useDashboard'
import DashboardStats from '@/components/dashboard/DashboardStats.vue'
import DashboardFilters from '@/components/dashboard/DashboardFilters.vue'

const {
  authStore,
  isLoading,
  errorMessage,
  projects,
  stats,
  filters,
  handleLogout,
  deleteProject
} = useDashboard()

const resetFilters = () => {
  filters.startDateFrom = ''
  filters.startDateTo = ''
  filters.priority = 1
  filters.sortBy = 'Name'
  filters.isDescending = false
}

</script>

<template>
  <div class="min-h-screen bg-slate-50 py-8 px-4 sm:px-6 lg:px-8">
    <div class="max-w-7xl mx-auto space-y-8">
      
      <div class="flex flex-col md:flex-row justify-between items-start md:items-center gap-4 bg-white p-6 rounded-2xl shadow-sm border border-slate-100">
        <div>
          <h1 class="text-2xl font-black text-slate-900 tracking-tight">Project Management System</h1>
          <p class="text-xs text-slate-500 mt-0.5">Logged in as: <span class="font-bold text-slate-700">{{ authStore.user?.email }}</span> ({{ authStore.user?.role }})</p>
        </div>

        <div class="flex flex-wrap items-center gap-3">
          <router-link 
            v-if="authStore.isDirector"
            to="/employees/create" 
            class="px-4 py-2 bg-slate-100 hover:bg-slate-200 text-slate-700 font-bold text-xs rounded-xl transition-all cursor-pointer"
          >
            👥 Employees Directory
          </router-link>
          
          <button 
            @click="handleLogout" 
            :disabled="isLoading"
            class="px-4 py-2 bg-rose-50 hover:bg-rose-100 text-rose-600 font-bold text-xs rounded-xl transition-all cursor-pointer disabled:opacity-50"
          >
            Sign Out
          </button>
        </div>
      </div>

      <DashboardStats :stats="stats" />

      <DashboardFilters :filters="filters" @reset="resetFilters" />

      <div class="bg-white rounded-2xl shadow-md border border-slate-100 overflow-hidden">
        <div class="p-6 border-b border-slate-50 flex justify-between items-center">
          <h2 class="text-lg font-extrabold text-slate-800">Active Projects List</h2>
          
          <router-link 
            v-if="authStore.isDirector"
            to="/projects/create" 
            class="px-4 py-2 bg-emerald-600 hover:bg-emerald-500 text-white font-bold text-xs rounded-xl shadow-md transition-all cursor-pointer"
          >
            + Create Project
          </router-link>
        </div>

        <div v-if="errorMessage" class="m-6 p-4 bg-red-50 border border-red-200 text-red-700 rounded-xl text-xs font-semibold">
          ⚠️ {{ errorMessage }}
        </div>

        <div v-if="isLoading && projects.length === 0" class="text-center py-16">
          <span class="text-slate-400 font-bold text-sm">Synchronizing with system...</span>
        </div>

        <div v-else-if="projects.length === 0" class="text-center py-16 text-slate-400 text-sm font-semibold">
          No projects match the selected filter criteria.
        </div>

        <div v-else class="overflow-x-auto">
          <table class="w-full text-left border-collapse">
            <thead>
              <tr class="bg-slate-50/70 border-b border-slate-100 text-[10px] font-bold text-slate-400 uppercase tracking-wider">
                <th class="px-6 py-4">Project Name</th>
                <th class="px-6 py-4">Customer</th>
                <th class="px-6 py-4">Executor</th>
                <th class="px-6 py-4">Timeline</th>
                <th class="px-6 py-4">Priority</th>
                <th class="px-6 py-4 text-right">Actions</th>
              </tr>
            </thead>
            <tbody class="divide-y divide-slate-50">
              <tr 
                v-for="project in projects" 
                :key="project.id" 
                class="hover:bg-slate-50/30 transition-colors"
              >
                <td class="px-6 py-4">
                  <div class="font-bold text-slate-800 text-sm">{{ project.name }}</div>
                </td>
                
                <td class="px-6 py-4 text-xs font-semibold text-slate-600">
                  {{ project.customerCompany }}
                </td>
                
                <td class="px-6 py-4 text-xs font-semibold text-slate-600">
                  {{ project.executorCompany }}
                </td>
                
                <td class="px-6 py-4 text-xs text-slate-500">
                  <div><span class="font-bold text-slate-400">Start:</span> {{ project.startDate }}</div>
                  <div v-if="project.endDate"><span class="font-bold text-slate-400">End:</span> {{ project.endDate }}</div>
                </td>
                
                <td class="px-6 py-4">
                  <span 
                    class="px-2.5 py-1 rounded-full text-[10px] font-bold"
                    :class="{
                      'bg-slate-100 text-slate-600': project.priority === 1,
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