<script setup lang="ts">

import type { Project } from '@/api/types' 

defineProps<{
  project: Project;
  isManagerOrDirector: boolean;
}>()

defineEmits<{
  (e: 'edit'): void
}>()
</script>

<template>
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
      <button 
        v-if="isManagerOrDirector"
        @click="$emit('edit')"
        class="px-4 py-2 bg-slate-100 hover:bg-slate-200 text-slate-700 text-xs font-bold rounded-xl transition-colors cursor-pointer flex items-center gap-1.5 h-fit self-end md:self-center"
      >
        <span>✏️ Edit Project</span>
      </button>
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
</template>