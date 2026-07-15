import { ref, reactive, watch, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import { ProjectsService } from '@/api/projects.service'
import type { Project, ProjectFilters } from '@/api/types'

export function useDashboard(){
    const authStore = useAuthStore()
    const router = useRouter()

    const isLoading = ref(false)
    const errorMessage = ref('')
    const projects = ref<Project[]>([])

    const stats = reactive({
        totalProjects: 0,
        highPriority: 0
    })

    const filters = reactive<ProjectFilters & { priority: number }>({
        startDateFrom: '',
        startDateTo: '',
        priority: 1,
        sortBy: 'Name',
        isDescending: false
    })

    // Logout logic
    const handleLogout = async () => {
        try{
            isLoading.value = true
            await authStore.logout()
            router.push({name: 'login'})
        }
        catch (err){
            console.error('Logout failed:', err)
        }
        finally{
            isLoading.value = false
        }
    }

    // Calculation of statistics based on the received projects
    const calculateStats = (projectsList: Project[]) => {
        stats.totalProjects = projectsList.length
        stats.highPriority = projectsList.filter(p => p.priority >= 3).length
    }

    // Loading projects from the backend via our API service
    const fetchProjects = async () =>{
        try{
            isLoading.value = true
            errorMessage.value = ''

            const cleanFilters: ProjectFilters = {
                startDateFrom: filters.startDateFrom || undefined,
                startDateTo: filters.startDateTo || undefined,
                priority: filters.priority !== 1 ? Number(filters.priority) : undefined,
                sortBy: filters.sortBy,
                isDescending: filters.isDescending
            }
            const data = await ProjectsService.getAll(cleanFilters)
            projects.value = data
            calculateStats(data)
        }
        catch (err: any){
            console.error('Failed to load projects:', err)
            errorMessage.value = 'Failed to load projects. Please try again.'
        }
        finally{
            isLoading.value = false
        }
    }

    // Deleting the project
    const deleteProject = async (id: number, name: string) => {
        if (!confirm(`Are you sure you want to delete project "${name}"?`)){
            return
        }
        try{
            isLoading.value = true
            await ProjectsService.delete(id)
            await fetchProjects()
        }
        catch (err: any){
            isLoading.value = true
            await ProjectsService.delete(id)
            await fetchProjects()
        }
        finally{
            isLoading.value = false
        }
    }  
    // Automatically re-fetch data when any filter changes
    watch(
        () => [filters.startDateFrom, filters.startDateTo, filters.priority, filters.sortBy, filters.isDescending],
        () => {
            fetchProjects()
        }
    )

    onMounted(() => {
        fetchProjects()
    })

    return {
        authStore,
        isLoading,
        errorMessage,
        projects,
        stats,
        filters,
        handleLogout,
        deleteProject,
        refetchProjects: fetchProjects
    }
}