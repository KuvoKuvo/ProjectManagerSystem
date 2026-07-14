<script setup lang="ts">

import { ref, watch } from 'vue'
import EmployeeSelector from './EmployeeSelector.vue'

interface Employee {
    id: number
    fullName: string
    email: string
}

interface Project {
    id: number
    name: string
    customerCompany: string
    executorCompany: string
    startDate?: string
    endDate?: string
    priority: number
    projectManagerId: number
    assignedEmployees?: Employee[]
}

const props = defineProps<{
    show: boolean
    project: Project | null
    allEmployees: Employee[]
    allManagers: any[]
    isDirector: boolean
    isSaving: boolean
}>()

const emit = defineEmits<{
    (e: 'close'): void
    (e: 'save', updateData: any): void
}>()

const editForm = ref({
    id: 0,
    name: '',
    customerCompany: '',
    executorCompany: '',
    startDate: '',
    endDate: '',
    priority: 3,
    projectManagerId: 0,
    employeeIds: [] as number[]
})

watch( () => props.show, (newVal) => {
    if(newVal && props.project) {
        editForm.value = {
            id: props.project.id,
            name: props.project.name,
            customerCompany: props.project.customerCompany,
            executorCompany: props.project.executorCompany,
            startDate: props.project.startDate?.split('T')[0] || '',
            endDate: props.project.endDate?.split('T')[0] || '',
            priority: props.project.priority,
            projectManagerId: props.project.projectManagerId,
            employeeIds: props.project.assignedEmployees?.map(e => e.id) || []
        }
    }
})

const submitForm = () => {
  if (
    !editForm.value.name.trim() || 
    !editForm.value.customerCompany.trim() || 
    !editForm.value.executorCompany.trim() || 
    !editForm.value.startDate
  ) {
    alert('Please fill out all required fields.')
    return
  }

  emit('save', { ...editForm.value })
}
</script>

<template>
  <div 
    v-if="show" 
    class="fixed inset-0 bg-slate-900/40 backdrop-blur-sm z-50 flex items-center justify-center p-4 overflow-y-auto"
  >
    <div class="bg-white rounded-2xl w-full max-w-lg shadow-xl border border-slate-100 flex flex-col my-8">
      
      <div class="px-6 py-4 border-b border-slate-100 flex items-center justify-between">
        <h3 class="text-base font-bold text-slate-800">Edit Baseline Information</h3>
        <button 
          @click="emit('close')" 
          type="button"
          class="text-slate-400 hover:text-slate-600 transition-colors cursor-pointer text-xl"
        >
          ✕
        </button>
      </div>

      <form @submit.prevent="submitForm" class="p-6 space-y-4 overflow-y-auto max-h-[calc(100vh-200px)]">
        
        <div class="space-y-1.5">
          <label class="block text-xs font-bold text-slate-500 uppercase">Project Name *</label>
          <input 
            v-model="editForm.name" 
            type="text" 
            required
            class="w-full bg-slate-50 border border-slate-200 rounded-xl p-2.5 text-sm outline-none focus:border-slate-400 transition-all"
          />
        </div>

        <div class="grid grid-cols-2 gap-4">
          <div class="space-y-1.5">
            <label class="block text-xs font-bold text-slate-500 uppercase">Customer *</label>
            <input 
              v-model="editForm.customerCompany" 
              type="text" 
              required
              class="w-full bg-slate-50 border border-slate-200 rounded-xl p-2.5 text-sm outline-none focus:border-slate-400 transition-all"
            />
          </div>
          <div class="space-y-1.5">
            <label class="block text-xs font-bold text-slate-500 uppercase">Executor *</label>
            <input 
              v-model="editForm.executorCompany" 
              type="text" 
              required
              class="w-full bg-slate-50 border border-slate-200 rounded-xl p-2.5 text-sm outline-none focus:border-slate-400 transition-all"
            />
          </div>
        </div>

        <div class="grid grid-cols-2 gap-4">
          <div class="space-y-1.5">
            <label class="block text-xs font-bold text-slate-500 uppercase">Start Date *</label>
            <input 
              v-model="editForm.startDate" 
              type="date" 
              required
              class="w-full bg-slate-50 border border-slate-200 rounded-xl p-2.5 text-sm outline-none focus:border-slate-400 transition-all"
            />
          </div>
          <div class="space-y-1.5">
            <label class="block text-xs font-bold text-slate-500 uppercase">End Date</label>
            <input 
              v-model="editForm.endDate" 
              type="date"
              class="w-full bg-slate-50 border border-slate-200 rounded-xl p-2.5 text-sm outline-none focus:border-slate-400 transition-all"
            />
          </div>
        </div>

        <div class="space-y-1.5">
          <label class="block text-xs font-bold text-slate-500 uppercase">Priority</label>
          <select 
            v-model="editForm.priority"
            class="w-full bg-slate-50 border border-slate-200 rounded-xl p-2.5 text-sm outline-none focus:border-slate-400 transition-all"
          >
            <option :value="1">1 (Low)</option>
            <option :value="2">2 (Medium-Low)</option>
            <option :value="3">3 (Medium)</option>
            <option :value="4">4 (High)</option>
            <option :value="5">5 (Critical)</option>
          </select>
        </div>

        <div v-if="isDirector" class="space-y-1.5">
          <label class="block text-xs font-bold text-slate-500 uppercase">Project Manager</label>
          <select 
            v-model="editForm.projectManagerId"
            class="w-full bg-slate-50 border border-slate-200 rounded-xl p-2.5 text-sm outline-none focus:border-slate-400 transition-all"
          >
            <option value="0" disabled>Select Project Manager...</option>
            <option v-for="m in allManagers" :key="m.id" :value="m.id">
              {{ m.fullName }} ({{ m.email }})
            </option>
          </select>
        </div>

        <EmployeeSelector 
          v-model="editForm.employeeIds" 
          :all-employees="allEmployees" 
        />

        <div class="flex items-center justify-end gap-3 pt-4 border-t border-slate-100">
          <button 
            @click="emit('close')" 
            type="button"
            class="px-4 py-2 text-sm font-medium text-slate-600 hover:bg-slate-50 rounded-xl transition-colors cursor-pointer"
          >
            Cancel
          </button>
          <button 
            type="submit"
            :disabled="isSaving"
            class="px-4 py-2 text-sm font-semibold text-white bg-slate-800 hover:bg-slate-700 disabled:bg-slate-300 rounded-xl transition-colors cursor-pointer"
          >
            {{ isSaving ? 'Saving...' : 'Save Changes' }}
          </button>
        </div>

      </form>
    </div>
  </div>
</template>