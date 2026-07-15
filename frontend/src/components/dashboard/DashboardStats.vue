<script setup lang="ts">

defineProps<{
    stats: {
        totalProjects: number
        highPriority: number
    }
}>()

</script>

<template>
  <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
    <div class="bg-white rounded-2xl p-6 shadow-md border border-slate-100 flex items-center justify-between">
      <div>
        <p class="text-xs font-bold text-slate-400 uppercase tracking-wider">Total Projects</p>
        <h3 class="text-3xl font-black text-slate-800 mt-1">{{ stats.totalProjects }}</h3>
      </div>
      <div class="w-12 h-12 bg-slate-100 rounded-xl flex items-center justify-center text-xl">
        📁
      </div>
    </div>

    <div class="bg-white rounded-2xl p-6 shadow-md border border-slate-100 flex items-center justify-between">
      <div>
        <p class="text-xs font-bold text-slate-400 uppercase tracking-wider">High Priority (Level >= 3)</p>
        <h3 class="text-3xl font-black text-rose-600 mt-1">{{ stats.highPriority }}</h3>
      </div>
      <div class="w-12 h-12 bg-rose-50 rounded-xl flex items-center justify-center text-xl">
        🔥
      </div>
    </div>
  </div>
</template>