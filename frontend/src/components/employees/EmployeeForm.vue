<script setup lang="ts">

import type { EmployeeCreatePayload } from '@/api/types'

defineProps<{
    form: EmployeeCreatePayload
    isEditMode: boolean
    isLoading: boolean
}>()

const emit = defineEmits<{
    (e: 'submit'): void
    (e: 'cancel'): void
}>()

</script>

<template>
  <div class="bg-white rounded-2xl p-6 shadow-md border border-slate-100">
    <h3 class="text-lg font-bold text-slate-800 mb-4">
      {{ isEditMode ? 'Edit Profile' : 'Register Employee' }}
    </h3>

    <form @submit.prevent="emit('submit')" class="space-y-4">
      <div>
        <label class="block text-xs font-semibold text-slate-500 uppercase tracking-wider mb-1">
          First Name *
        </label>
        <input 
          v-model="form.firstName" 
          type="text" 
          required 
          class="w-full rounded-xl border border-slate-200 px-3 py-2 text-sm focus:border-emerald-500 focus:outline-none bg-slate-50"
        />
      </div>

      <div>
        <label class="block text-xs font-semibold text-slate-500 uppercase tracking-wider mb-1">
          Last Name *
        </label>
        <input 
          v-model="form.lastName" 
          type="text" 
          required 
          class="w-full rounded-xl border border-slate-200 px-3 py-2 text-sm focus:border-emerald-500 focus:outline-none bg-slate-50"
        />
      </div>

      <div>
        <label class="block text-xs font-semibold text-slate-500 uppercase tracking-wider mb-1">
          Middle Name (Optional)
        </label>
        <input 
          v-model="form.middleName" 
          type="text" 
          class="w-full rounded-xl border border-slate-200 px-3 py-2 text-sm focus:border-emerald-500 focus:outline-none bg-slate-50"
        />
      </div>

      <div>
        <label class="block text-xs font-semibold text-slate-500 uppercase tracking-wider mb-1">
          Email *
        </label>
        <input 
          v-model="form.email" 
          type="email" 
          required 
          class="w-full rounded-xl border border-slate-200 px-3 py-2 text-sm focus:border-emerald-500 focus:outline-none bg-slate-50"
        />
      </div>

      <div>
        <label class="block text-xs font-semibold text-slate-500 uppercase tracking-wider mb-1">
          System Role *
        </label>
        <select 
          v-model="form.role" 
          required 
          class="w-full rounded-xl border border-slate-200 px-3 py-2 text-sm focus:border-emerald-500 focus:outline-none bg-slate-50"
        >
          <option value="Employee">Employee</option>
          <option value="ProjectManager">Project Manager</option>
          <option value="Director">Director</option>
        </select>
      </div>

      <div class="space-y-2 pt-2">
        <button 
          type="submit" 
          :disabled="isLoading" 
          class="w-full py-2.5 bg-emerald-600 hover:bg-emerald-500 text-white font-semibold text-sm rounded-xl transition-all shadow-md cursor-pointer disabled:opacity-50"
        >
          <span v-if="isLoading">Processing API Request...</span>
          <span v-else>{{ isEditMode ? 'Apply Updates' : 'Create Account' }}</span>
        </button>
        
        <button 
          v-if="isEditMode"
          type="button" 
          @click="emit('cancel')" 
          class="w-full py-2.5 bg-slate-100 hover:bg-slate-200 text-slate-700 font-semibold text-xs rounded-xl transition-all cursor-pointer"
        >
          Cancel Edit
        </button>
      </div>
    </form>
  </div>
</template>