<script setup lang="ts">
import { ref } from 'vue'
import type { Project } from '@/api/types'

const props = defineProps<{
  editForm: {
    id: number;
    name: string;
    customerCompany: string;
    executorCompany: string;
    startDate: string;
    endDate: string | null;
    priority: number;
    projectManagerId: number;
    employeeIds: number[];
  };
  isSaving: boolean;
  project: Project;
  allManagers: Array<{ id: number; fullName: string }>;
  allEmployees: Array<{ id: number; fullName: string; email: string }>;
  isDirector: boolean;
}>()

const emit = defineEmits<{
  (e: 'update:editForm', value: any): void
  (e: 'close'): void
  (e: 'save'): void
  (e: 'add-employee', empId: number): void
  (e: 'remove-employee', empId: number): void
}>()

const selectedEmployeeToAdd = ref<number | ''>('')

const updateField = (field: keyof typeof props.editForm, value: any) => {
  emit('update:editForm', { ...props.editForm, [field]: value })
}

const handleAddEmployee = () => {
  if (selectedEmployeeToAdd.value) {
    emit('add-employee', Number(selectedEmployeeToAdd.value))
    selectedEmployeeToAdd.value = '' // Сбрасываем селект после добавления
  }
}
</script>

<template>
  <div class="fixed inset-0 bg-slate-900/40 backdrop-blur-sm z-50 flex items-center justify-center p-4">
    <div class="bg-white rounded-2xl max-w-2xl w-full shadow-2xl overflow-hidden flex flex-col max-h-[90vh]">
      
      <div class="p-5 border-b border-slate-100 flex justify-between items-center bg-slate-50/50">
        <h3 class="text-lg font-bold text-slate-800">Edit Project Details</h3>
        <button @click="$emit('close')" class="text-slate-400 hover:text-slate-600 transition-colors p-1 cursor-pointer">
          ✖
        </button>
      </div>

      <div class="p-6 overflow-y-auto space-y-6">
        
        <div class="space-y-1.5">
          <label class="text-xs font-bold text-slate-500 uppercase tracking-wider">Project Name</label>
          <input 
            :value="editForm.name"
            @input="updateField('name', ($event.target as HTMLInputElement).value)"
            type="text" 
            class="w-full border border-slate-200 rounded-xl px-4 py-2.5 text-sm focus:ring-2 focus:ring-slate-900 outline-none"
          />
        </div>

        <div class="grid grid-cols-1 sm:grid-cols-2 gap-5">
          <div class="space-y-1.5">
            <label class="text-xs font-bold text-slate-500 uppercase tracking-wider">Customer Company</label>
            <input 
              :value="editForm.customerCompany"
              @input="updateField('customerCompany', ($event.target as HTMLInputElement).value)"
              type="text" 
              class="w-full border border-slate-200 rounded-xl px-4 py-2.5 text-sm focus:ring-2 focus:ring-slate-900 outline-none"
            />
          </div>
          <div class="space-y-1.5">
            <label class="text-xs font-bold text-slate-500 uppercase tracking-wider">Executing Company</label>
            <input 
              :value="editForm.executorCompany"
              @input="updateField('executorCompany', ($event.target as HTMLInputElement).value)"
              type="text" 
              class="w-full border border-slate-200 rounded-xl px-4 py-2.5 text-sm focus:ring-2 focus:ring-slate-900 outline-none"
            />
          </div>
        </div>

        <div class="grid grid-cols-1 sm:grid-cols-3 gap-5 border-y border-slate-100 py-5">
          <div class="space-y-1.5">
            <label class="text-xs font-bold text-slate-500 uppercase tracking-wider">Start Date</label>
            <input 
              :value="editForm.startDate?.split('T')[0]"
              @input="updateField('startDate', ($event.target as HTMLInputElement).value)"
              type="date" 
              class="w-full border border-slate-200 rounded-xl px-3 py-2 text-sm focus:ring-2 focus:ring-slate-900 outline-none"
            />
          </div>
          <div class="space-y-1.5">
            <label class="text-xs font-bold text-slate-500 uppercase tracking-wider">End Date</label>
            <input 
              :value="editForm.endDate?.split('T')[0] || ''"
              @input="updateField('endDate', ($event.target as HTMLInputElement).value || null)"
              type="date" 
              class="w-full border border-slate-200 rounded-xl px-3 py-2 text-sm focus:ring-2 focus:ring-slate-900 outline-none"
            />
            <p v-if="editForm.endDate && editForm.startDate && new Date(editForm.endDate) < new Date(editForm.startDate)" class="text-[10px] text-red-500 font-bold">
              ⚠️ End date cannot be before start date
            </p>
          </div>
          <div class="space-y-1.5">
            <label class="text-xs font-bold text-slate-500 uppercase tracking-wider">Priority</label>
            <select 
              :value="editForm.priority"
              @change="updateField('priority', Number(($event.target as HTMLSelectElement).value))"
              class="w-full border border-slate-200 rounded-xl px-3 py-2 text-sm focus:ring-2 focus:ring-slate-900 outline-none bg-white"
            >
              <option :value="1">1 - Lowest</option>
              <option :value="2">2 - Low</option>
              <option :value="3">3 - Medium</option>
              <option :value="4">4 - High</option>
              <option :value="5">5 - Highest</option>
            </select>
          </div>
        </div>

        <div v-if="isDirector" class="space-y-1.5">
          <label class="text-xs font-bold text-slate-500 uppercase tracking-wider">Project Manager</label>
          <select 
            :value="editForm.projectManagerId"
            @change="updateField('projectManagerId', Number(($event.target as HTMLSelectElement).value))"
            class="w-full border border-slate-200 rounded-xl px-4 py-2.5 text-sm focus:ring-2 focus:ring-slate-900 outline-none bg-white"
          >
            <option :value="0">No Manager</option>
            <option v-for="mgr in allManagers" :key="mgr.id" :value="mgr.id">
              {{ mgr.fullName }}
            </option>
          </select>
        </div>

        <div class="space-y-3">
          <h4 class="text-sm font-bold text-slate-800">Project Executors</h4>
          
          <div class="flex gap-2">
            <select 
              v-model="selectedEmployeeToAdd" 
              class="flex-1 border border-slate-200 rounded-xl px-3 py-2 text-sm outline-none"
            >
              <option value="" disabled>Select employee to add...</option>
              <option v-for="emp in allEmployees.filter(e => !editForm.employeeIds.includes(e.id))" :key="emp.id" :value="emp.id">
                {{ emp.fullName }}
              </option>
            </select>
            <button 
              type="button"
              @click="handleAddEmployee"
              :disabled="!selectedEmployeeToAdd"
              class="px-4 py-2 bg-slate-100 hover:bg-slate-200 text-slate-700 font-bold text-sm rounded-xl disabled:opacity-50 transition-colors"
            >
              Add
            </button>
          </div>

          <div v-if="editForm.employeeIds.length > 0" class="border border-slate-200 rounded-xl divide-y divide-slate-100 max-h-40 overflow-y-auto">
            <div 
              v-for="empId in editForm.employeeIds" 
              :key="empId"
              class="flex justify-between items-center px-4 py-2 hover:bg-slate-50 transition-colors"
            >
              <div class="text-sm">
                <p class="font-bold text-slate-700">{{ allEmployees.find(e => e.id === empId)?.fullName }}</p>
                <p class="text-[10px] text-slate-400">{{ allEmployees.find(e => e.id === empId)?.email }}</p>
              </div>
              <button 
                type="button"
                @click="$emit('remove-employee', empId)"
                class="text-xs text-red-500 hover:text-red-700 font-bold px-2 py-1 hover:bg-red-50 rounded-lg transition-colors cursor-pointer"
              >
                Remove
              </button>
            </div>
          </div>
          <div v-else class="text-center py-6 bg-slate-50 rounded-xl border border-dashed border-slate-200">
            <p class="text-xs text-slate-400">No executors assigned to this project yet.</p>
          </div>
        </div>

      </div>

      <div class="p-5 border-t border-slate-100 bg-slate-50/50 flex justify-end gap-3">
        <button 
          @click="$emit('close')"
          class="px-4 py-2 text-sm font-bold text-slate-600 hover:bg-slate-200 bg-slate-100 rounded-xl transition-colors cursor-pointer"
        >
          Cancel
        </button>
        <button 
          @click="$emit('save')"
          :disabled="isSaving || (!!editForm.endDate && new Date(editForm.endDate) < new Date(editForm.startDate))"
          class="px-4 py-2 text-sm font-bold text-white bg-slate-900 hover:bg-slate-800 rounded-xl transition-colors disabled:opacity-50 disabled:cursor-not-allowed cursor-pointer"
        >
          {{ isSaving ? 'Saving...' : 'Save Changes' }}
        </button>
      </div>
    </div>
  </div>
</template>