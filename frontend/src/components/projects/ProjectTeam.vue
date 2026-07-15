<script setup lang="ts">

defineProps<{
  projectManager?: { 
    id?: number; 
    fullName: string; 
    email: string 
  } | null;
  
  employees: Array<{ 
    id: number; 
    fullName: string; 
    email: string 
  }>;
}>()
</script>

<template>
  <div class="bg-white rounded-2xl p-6 shadow-sm border border-slate-100 space-y-6">
    <h2 class="text-xl font-bold text-slate-900 border-b border-slate-100 pb-4">Project Team</h2>

    <div class="space-y-3">
      <h3 class="text-xs font-bold text-slate-400 uppercase tracking-wider">Project Manager</h3>
      <div v-if="projectManager" class="flex items-center gap-3 p-3 bg-slate-50 rounded-xl border border-slate-100">
        <span class="text-2xl">👑</span>
        <div>
          <p class="text-sm font-bold text-slate-800">{{ projectManager.fullName }}</p>
          <p class="text-xs text-slate-400">{{ projectManager.email }}</p>
        </div>
      </div>
      <div v-else class="text-sm text-amber-600 bg-amber-50 p-3 rounded-xl border border-amber-200">
        ⚠️ No manager is assigned to the project!
      </div>
    </div>

    <div class="space-y-3">
      <h3 class="text-xs font-bold text-slate-400 uppercase tracking-wider">Executors ({{ employees.length }})</h3>
      <div v-if="employees.length > 0" class="space-y-2">
        <div 
          v-for="emp in employees" 
          :key="emp.id" 
          class="flex items-center gap-3 p-3 hover:bg-slate-50 rounded-xl border border-transparent hover:border-slate-100 transition-all"
        >
          <span class="text-xl">👤</span>
          <div>
            <p class="text-sm font-bold text-slate-800">{{ emp.fullName }}</p>
            <p class="text-xs text-slate-400">{{ emp.email }}</p>
          </div>
        </div>
      </div>
      <div v-else class="text-sm text-slate-400 text-center py-6 border-2 border-dashed border-slate-100 rounded-xl">
        No employees are assigned to this project yet.
      </div>
    </div>
  </div>
</template>