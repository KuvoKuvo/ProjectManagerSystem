<script setup lang="ts">

import type { ProjectFilters } from '@/api/types'

defineProps<{
    filters: ProjectFilters & { priority: number }
}>()

const emit = defineEmits<{
    (e: 'reset'): void
}>()

const handleReset = () => {
    emit('reset')
}

</script>

<template>
  <div class="bg-white rounded-2xl p-6 shadow-md border border-slate-100 space-y-4">
    <div class="flex justify-between items-center border-b border-slate-50 pb-3">
      <h3 class="text-sm font-bold text-slate-800 flex items-center gap-2">
        <span>⚙️</span> Filters & Sorting
      </h3>
      <button 
        type="button"
        @click="handleReset"
        class="text-xs text-rose-500 hover:text-rose-700 font-bold px-2 py-1 hover:bg-rose-50 rounded-lg transition-colors cursor-pointer"
      >
        Reset Filters
      </button>
    </div>

    <div class="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-5 gap-4">
      <div>
        <label class="block text-[10px] font-bold text-slate-400 uppercase tracking-wider mb-1">
          Date From
        </label>
        <input 
          v-model="filters.startDateFrom" 
          type="date" 
          class="w-full rounded-xl border border-slate-200 px-3 py-1.5 text-xs focus:border-emerald-500 focus:outline-none bg-slate-50 text-slate-700"
        />
      </div>

      <div>
        <label class="block text-[10px] font-bold text-slate-400 uppercase tracking-wider mb-1">
          Date To
        </label>
        <input 
          v-model="filters.startDateTo" 
          type="date" 
          class="w-full rounded-xl border border-slate-200 px-3 py-1.5 text-xs focus:border-emerald-500 focus:outline-none bg-slate-50 text-slate-700"
        />
      </div>

      <div>
        <label class="block text-[10px] font-bold text-slate-400 uppercase tracking-wider mb-1">
          Priority (1-5)
        </label>
        <select 
          v-model="filters.priority" 
          class="w-full rounded-xl border border-slate-200 px-3 py-1.5 text-xs focus:border-emerald-500 focus:outline-none bg-slate-50 text-slate-700"
        >
          <option :value="0">All Priorities</option>
          <option :value="1">Level 1</option>
          <option :value="2">Level 2</option>
          <option :value="3">Level 3</option>
          <option :value="4">Level 4</option>
          <option :value="5">Level 5</option>
        </select>
      </div>

      <div>
        <label class="block text-[10px] font-bold text-slate-400 uppercase tracking-wider mb-1">
          Sort By
        </label>
        <select 
          v-model="filters.sortBy" 
          class="w-full rounded-xl border border-slate-200 px-3 py-1.5 text-xs focus:border-emerald-500 focus:outline-none bg-slate-50 text-slate-700"
        >
          <option value="Name">Project Name</option>
          <option value="StartDate">Start Date</option>
          <option value="Priority">Priority</option>
        </select>
      </div>

      <div>
        <label class="block text-[10px] font-bold text-slate-400 uppercase tracking-wider mb-1">
          Direction
        </label>
        <select 
          v-model="filters.isDescending" 
          class="w-full rounded-xl border border-slate-200 px-3 py-1.5 text-xs focus:border-emerald-500 focus:outline-none bg-slate-50 text-slate-700"
        >
          <option :value="false">Ascending ↑</option>
          <option :value="true">Descending ↓</option>
        </select>
      </div>
    </div>
  </div>
</template>