<script setup lang="ts">
import { useEmployees } from '@/composables/useEmployees'
import EmployeeForm from '@/components/employees/EmployeeForm.vue'

const {
  isLoading,
  listLoading,
  globalError,
  successMessage,
  searchQuery,
  generatedPassword,
  isCopied,
  isEditMode,
  form,
  filteredEmployees,
  copyPassword,
  startEdit,
  cancelEdit,
  submitForm,
  deleteEmployee
} = useEmployees()
</script>

<template>
  <div class="min-h-screen bg-slate-50 py-8 px-4 sm:px-6 lg:px-8">
    <div class="max-w-7xl mx-auto space-y-8">
      
      <div class="flex flex-col md:flex-row justify-between items-start md:items-center gap-4">
        <div>
          <h1 class="text-3xl font-extrabold text-slate-900 tracking-tight">Employees Management</h1>
          <p class="text-sm text-slate-500 mt-1">Manage corporate staff accounts, roles, and credentials</p>
        </div>
        <router-link 
          to="/" 
          class="px-4 py-2 bg-slate-900 hover:bg-slate-800 text-white font-semibold text-sm rounded-xl transition-all shadow-sm cursor-pointer"
        >
          ← Back to Dashboard
        </router-link>
      </div>

      <div class="grid grid-cols-1 lg:grid-cols-3 gap-8">
        
        <div class="lg:col-span-2 space-y-6">
          <div class="bg-white rounded-2xl p-6 shadow-md border border-slate-100">
            <div class="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4 mb-6">
              <h2 class="text-xl font-bold text-slate-800">Team Directory</h2>
              
              <div class="relative w-full sm:w-64">
                <input 
                  v-model="searchQuery" 
                  type="text" 
                  placeholder="Search name, email, role..."   
                  class="w-full pl-9 pr-4 py-2 border border-slate-200 rounded-xl text-sm focus:border-emerald-500 focus:outline-none bg-slate-50"
                />
                <span class="absolute left-3 top-2.5 text-slate-400 text-sm">🔍</span>
              </div>
            </div>

            <div v-if="listLoading" class="text-center py-12">
              <span class="text-slate-500 font-medium">Fetching directory...</span>
            </div>

            <div v-else-if="filteredEmployees.length === 0" class="text-center py-12 text-slate-400">
              No employees found matching the criteria.
            </div>

            <div v-else class="overflow-x-auto">
              <table class="w-full text-left border-collapse">
                <thead>
                  <tr class="border-b border-slate-100">
                    <th class="pb-3 text-xs font-semibold text-slate-400 uppercase tracking-wider">Employee</th>
                    <th class="pb-3 text-xs font-semibold text-slate-400 uppercase tracking-wider">Role</th>
                    <th class="pb-3 text-xs font-semibold text-slate-400 uppercase tracking-wider text-right">Actions</th>
                  </tr>
                </thead>
                <tbody class="divide-y divide-slate-50">
                  <tr v-for="emp in filteredEmployees" :key="emp.id" class="hover:bg-slate-50/50 transition-colors">
                    <td class="py-4">
                      <div class="font-bold text-slate-800">{{ emp.lastName }} {{ emp.firstName }} {{ emp.middleName }}</div>
                      <div class="text-xs text-slate-400">{{ emp.email }}</div>
                    </td>
                    <td class="py-4">
                      <span 
                        class="px-2.5 py-1 rounded-full text-xs font-bold"
                        :class="{
                          'bg-emerald-50 text-emerald-700': emp.role === 'Director',
                          'bg-indigo-50 text-indigo-700': emp.role === 'ProjectManager',
                          'bg-slate-100 text-slate-600': emp.role === 'Employee'
                        }"
                      >
                        {{ emp.role }}
                      </span>
                    </td>
                    <td class="py-4 text-right">
                      <div class="flex items-center justify-end gap-2">
                        <button 
                          @click="startEdit(emp)" 
                          class="px-3 py-1.5 bg-slate-100 hover:bg-slate-200 text-slate-700 rounded-lg text-xs font-bold transition-colors cursor-pointer"
                        >
                          Edit
                        </button>
                        <button 
                          @click="deleteEmployee(emp.id, `${emp.firstName} ${emp.lastName}`)" 
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

        <div class="space-y-6">
          <div 
            v-if="generatedPassword" 
            class="bg-amber-50 border border-amber-200 rounded-2xl p-6 space-y-4 shadow-sm"
          >
            <div class="flex items-start gap-3">
              <span class="text-2xl">🗝️</span>
              <div>
                <h4 class="font-bold text-amber-900 text-sm">Temporary Password Generated</h4>
                <p class="text-xs text-amber-700 mt-1">
                  Copy and send this password to the employee. It will not be shown again!
                </p>
              </div>
            </div>

            <div class="flex gap-2">
              <input 
                :value="generatedPassword" 
                readonly 
                type="text" 
                class="w-full bg-white border border-amber-200 rounded-xl px-3 py-2 text-sm font-mono text-amber-950 focus:outline-none"
              />
              <button 
                @click="copyPassword" 
                class="px-4 py-2 bg-amber-600 hover:bg-amber-700 text-white font-semibold text-xs rounded-xl transition-all cursor-pointer whitespace-nowrap"
              >
                {{ isCopied ? 'Copied!' : 'Copy' }}
              </button>
            </div>
          </div>

          <div v-if="globalError" class="p-4 bg-red-50 border border-red-200 text-red-700 rounded-xl text-sm font-semibold">
            ⚠️ {{ globalError }}
          </div>

          <div v-if="successMessage" class="p-4 bg-emerald-50 border border-emerald-200 text-emerald-700 rounded-xl text-sm font-semibold">
            ✅ {{ successMessage }}
          </div>

          <EmployeeForm 
            :form="form" 
            :isEditMode="isEditMode" 
            :isLoading="isLoading" 
            @submit="submitForm" 
            @cancel="cancelEdit"
          />
        </div>

      </div>
    </div>
  </div>
</template>