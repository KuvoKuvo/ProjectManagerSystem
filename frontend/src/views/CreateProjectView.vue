<script setup lang="ts">

import { useCreateProject } from '@/composables/useCreateProject'
import Step3Manager from '@/components/projects/WizardSteps/Step3Manager.vue'
import Step4Employees from '@/components/projects/WizardSteps/Step4Employees.vue'
import Step5Files from '@/components/projects/WizardSteps/Step5Files.vue'

const {
  currStep,
  isLoading,
  globalError,
  wizardData,
  managersList,
  searchQuery,
  searchResult,
  isSearching,
  isNextButtonActive,
  nextStep,
  prevStep,
  selectManager,
  handleSearchInput,
  addEmployee,
  removeEmployee,
  handleFileChange,
  removeFile,
  submitProject
} = useCreateProject()

</script>

<template>
  <div class="min-h-screen bg-slate-50 py-12 px-4 sm:px-6 lg:px-8">
    <div class="max-w-2xl mx-auto bg-white rounded-3xl p-8 shadow-xl border border-slate-100">
      
      <div class="space-y-4 mb-8">
        <div class="flex justify-between items-center">
          <div>
            <h1 class="text-2xl font-black text-slate-900 tracking-tight">Create New Project</h1>
            <p class="text-xs text-slate-500 mt-1">Create project through our 5-step wizard setup</p>
          </div>
          <router-link 
            to="/" 
            class="text-xs font-bold text-slate-400 hover:text-slate-600 transition-colors"
          >
            Exit wizard
          </router-link>
        </div>

        <div class="flex items-center gap-2 pt-2">
          <div 
            v-for="step in 5" 
            :key="step"
            class="h-1.5 flex-1 rounded-full transition-all duration-300"
            :class="step <= currStep ? 'bg-emerald-500' : 'bg-slate-100'"
          />
        </div>
        <p class="text-[10px] font-bold text-slate-400 uppercase tracking-wider">
          Step {{ currStep }} of 5
        </p>
      </div>

      <form @submit.prevent>
        
        <div v-if="currStep === 1" class="space-y-4 animate-fade-in">
          <div>
            <label class="block text-xs font-semibold text-slate-500 uppercase tracking-wider mb-1">
              Project Name *
            </label>
            <input 
              v-model="wizardData.name" 
              type="text" 
              required  
              class="w-full rounded-xl border border-slate-200 px-3 py-2 text-sm focus:border-emerald-500 focus:outline-none bg-slate-50"
            />
          </div>

          <div class="grid grid-cols-2 gap-4">
            <div>
              <label class="block text-xs font-semibold text-slate-500 uppercase tracking-wider mb-1">
                Start Date *
              </label>
              <input 
                v-model="wizardData.startDate" 
                type="date" 
                required 
                class="w-full rounded-xl border border-slate-200 px-3 py-2 text-sm focus:border-emerald-500 focus:outline-none bg-slate-50 text-slate-700"
              />
            </div>
            <div>
              <label class="block text-xs font-semibold text-slate-500 uppercase tracking-wider mb-1">
                End Date (Optional)
              </label>
              <input 
                v-model="wizardData.endDate" 
                type="date" 
                class="w-full rounded-xl border border-slate-200 px-3 py-2 text-sm focus:border-emerald-500 focus:outline-none bg-slate-50 text-slate-700"
              />
              <p v-if="wizardData.endDate && wizardData.startDate && new Date(wizardData.endDate) < new Date(wizardData.startDate)" class="text-[11px] text-red-500 mt-1 font-semibold">
                ⚠️ End date cannot be earlier than start date.
              </p>
            </div>
          </div>

          <div>
            <label class="block text-xs font-semibold text-slate-500 uppercase tracking-wider mb-1">
              Priority Level (1-5) *
            </label>
            <div class="flex items-center gap-3">
              <input 
                v-model="wizardData.priority" 
                type="range" 
                min="1" 
                max="5" 
                class="w-full accent-emerald-500"
              />
              <span class="text-sm font-bold text-slate-700 w-6 text-right">{{ wizardData.priority }}</span>
            </div>
          </div>
        </div>

        <div v-if="currStep === 2" class="space-y-4 animate-fade-in">
          <div>
            <label class="block text-xs font-semibold text-slate-500 uppercase tracking-wider mb-1">
              Customer Company *
            </label>
            <input 
              v-model="wizardData.customerCompany" 
              type="text" 
              required 
              class="w-full rounded-xl border border-slate-200 px-3 py-2 text-sm focus:border-emerald-500 focus:outline-none bg-slate-50"
            />
          </div>

          <div>
            <label class="block text-xs font-semibold text-slate-500 uppercase tracking-wider mb-1">
              Executor Company *
            </label>
            <input 
              v-model="wizardData.executorCompany" 
              type="text" 
              required 
              class="w-full rounded-xl border border-slate-200 px-3 py-2 text-sm focus:border-emerald-500 focus:outline-none bg-slate-50"
            />
          </div>
        </div>

        <Step3Manager 
          v-if="currStep === 3"
          :managersList="managersList"
          :selectedId="wizardData.projectManagerId"
          @select="selectManager"
        />

        <Step4Employees 
          v-if="currStep === 4"
          v-model:searchQuery="searchQuery"
          :searchResult="searchResult"
          :selectedEmployees="wizardData.employees"
          :isSearching="isSearching"
          @search-input="handleSearchInput"
          @add="addEmployee"
          @remove="removeEmployee"
        />

        <Step5Files 
          v-if="currStep === 5"
          :files="wizardData.files"
          @file-change="handleFileChange"
          @remove-file="removeFile"
        />

        <div v-if="globalError" class="mt-6 p-4 bg-red-50 border border-red-200 text-red-700 rounded-xl text-xs font-semibold">
          ⚠️ {{ globalError }}
        </div>

        <div class="mt-8 pt-6 border-t border-slate-100 flex justify-between items-center">
          <button 
            type="button" 
            @click="prevStep" 
            v-if="currStep > 1"
            :disabled="isLoading"
            class="px-5 py-2.5 border border-slate-300 rounded-xl text-slate-700 text-xs font-semibold hover:bg-slate-50 transition-colors cursor-pointer disabled:opacity-50"
          >
            Back
          </button>
          <div v-else></div> 

          <button 
            type="button" 
            @click="nextStep" 
            v-if="currStep < 5"
            :disabled="!isNextButtonActive"
            class="px-5 py-2.5 bg-slate-900 text-white rounded-xl text-xs font-semibold hover:bg-slate-800 transition-colors cursor-pointer disabled:opacity-40 disabled:cursor-not-allowed"
          >
            Next →
          </button>

          <button 
            type="button" 
            @click="submitProject" 
            v-if="currStep === 5"
            :disabled="isLoading"
            class="px-6 py-2.5 bg-emerald-600 text-white rounded-xl text-xs font-semibold hover:bg-emerald-500 shadow-md transition-colors cursor-pointer disabled:opacity-50"
          >
            <span v-if="isLoading">Uploading and Saving...</span>
            <span v-else>Launch Project 🚀</span>
          </button>
        </div>
      </form>
    </div>
  </div>
</template>