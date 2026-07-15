<script setup lang="ts">
    defineProps<{
    managersList: Array<{ id: number; fullName: string }>
    selectedId: number | null
}>()

const emit = defineEmits<{
    (e: 'select', id: number, name: string): void
}>()
</script>   

<template>
  <div class="space-y-4 animate-fade-in">
    <div class="text-center max-w-md mx-auto mb-6">
      <h3 class="text-lg font-bold text-slate-800">Assign a Project Manager</h3>
      <p class="text-xs text-slate-500 mt-1">
        Select a leader who will oversee the development process and tasks distribution.
      </p>
    </div>

    <div v-if="managersList.length === 0" class="text-center py-6 text-slate-400">
      Loading available managers...
    </div>

    <div v-else class="grid grid-cols-1 md:grid-cols-2 gap-3 max-h-60 overflow-y-auto p-1">
      <button 
        v-for="manager in managersList" 
        :key="manager.id"
        type="button"
        @click="emit('select', manager.id, manager.fullName)"
        class="flex items-center gap-3 p-3.5 border rounded-xl text-left transition-all cursor-pointer"
        :class="selectedId === manager.id 
          ? 'border-emerald-500 bg-emerald-50/50 shadow-sm ring-1 ring-emerald-500' 
          : 'border-slate-200 hover:border-slate-300 hover:bg-slate-50'"
      >
        <span class="w-8 h-8 rounded-full bg-slate-100 flex items-center justify-center text-sm font-bold text-slate-600">
          💼
        </span>
        <div>
          <p class="text-sm font-bold text-slate-800">{{ manager.fullName }}</p>
          <p class="text-[10px] text-slate-400">Project Manager</p>
        </div>
      </button>
    </div>
  </div>
</template>