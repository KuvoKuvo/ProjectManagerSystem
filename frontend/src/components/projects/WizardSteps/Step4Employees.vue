<script setup lang="ts">

defineProps<{
    searchQuery: string
    searchResult: Array<{ id: number; fullName: string }>
    selectedEmployees: Array<{ id: number; fullName: string }>
    isSearching: boolean
}>()

const emit = defineEmits<{
    (e: 'update:searchQuery', value: string): void
    (e: 'search-input'): void
    (e: 'add', emp: { id: number; fullName: string }): void
    (e: 'remove', id: number): void
}>()

</script>

<template>
  <div class="space-y-6 animate-fade-in">
    <div class="text-center max-w-md mx-auto">
      <h3 class="text-lg font-bold text-slate-800">Assemble Project Team</h3>
      <p class="text-xs text-slate-500 mt-1">
        Search and assign engineers, designers and other staff members to this project.
      </p>
    </div>

    <div class="relative max-w-md mx-auto">
      <label class="block text-xs font-semibold text-slate-500 uppercase tracking-wider mb-1">
        Find employee
      </label>
      <div class="relative">
        <input 
          :value="searchQuery"
          @input="emit('update:searchQuery', ($event.target as HTMLInputElement).value); emit('search-input')"
          type="text" 
          placeholder="Type name or surname..." 
          class="w-full pl-9 pr-4 py-2.5 border border-slate-200 rounded-xl text-sm focus:border-emerald-500 focus:outline-none bg-slate-50"
        />
        <span class="absolute left-3 top-3 text-slate-400 text-sm">🔍</span>
        <span v-if="isSearching" class="absolute right-3 top-3 text-xs text-slate-400">Searching...</span>
      </div>

      <div 
        v-if="searchResult.length > 0" 
        class="absolute z-10 w-full mt-1 bg-white border border-slate-200 rounded-xl shadow-lg max-h-48 overflow-y-auto divide-y divide-slate-50"
      >
        <button 
          v-for="emp in searchResult" 
          :key="emp.id"
          type="button"
          @click="emit('add', emp)"
          class="w-full text-left px-4 py-2.5 text-xs text-slate-700 hover:bg-slate-50 font-semibold flex items-center justify-between cursor-pointer"
        >
          <span>{{ emp.fullName }}</span>
          <span class="text-emerald-600 font-bold">+ Assign</span>
        </button>
      </div>
    </div>

    <div class="max-w-md mx-auto">
      <label class="block text-xs font-semibold text-slate-500 uppercase tracking-wider mb-2">
        Team Members ({{ selectedEmployees.length }})
      </label>

      <div v-if="selectedEmployees.length === 0" class="text-center py-6 bg-slate-50 rounded-xl border border-dashed border-slate-200">
        <p class="text-xs text-slate-400">No employees assigned yet. Find and add some above.</p>
      </div>

      <div v-else class="max-h-48 overflow-y-auto space-y-2 pr-1">
        <div 
          v-for="emp in selectedEmployees" 
          :key="emp.id"
          class="flex items-center justify-between p-2.5 bg-slate-50 border border-slate-100 rounded-xl"
        >
          <span class="text-xs font-bold text-slate-700">{{ emp.fullName }}</span>
          <button 
            type="button"
            @click="emit('remove', emp.id)"
            class="text-xs text-rose-500 hover:text-rose-700 font-bold px-2 py-1 hover:bg-rose-50 rounded-lg transition-colors cursor-pointer"
          >
            Remove
          </button>
        </div>
      </div>
    </div>
  </div>
</template>