import { ref, reactive, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { ProjectsService } from '@/api/projects.service'
import { EmployeesService } from '@/api/employees.service'
import type { Employee } from '@/api/types'

export function useCreateProject(){
    const router = useRouter()
    const currStep = ref(1)
    const isLoading = ref(false)
    const globalError = ref('')

    const wizardData = reactive({
        name: '',
        startDate: '',
        endDate: '',
        priority: 1,
        customerCompany: '',
        executorCompany: '',
        projectManagerId: null as number | null,
        projectManagerName: '',
        employees: [] as Array<{ id: number; fullName: string }>,
        files: [] as File[]
    })

    const managersList = ref<Array<{ id: number; fullName: string }>>([])

    const searchQuery = ref('')
    const searchResult = ref<Array<{ id: number; fullName: string }>>([])
    const isSearching = ref(false)
    let debounceTimeout: ReturnType<typeof setTimeout>

    // Step-by-Step Logic
    const nextStep = () => {
        if (currStep.value < 5) currStep.value++
    }

    const prevStep = () => {
        if (currStep.value > 1) currStep.value--
    }

    const isNextButtonActive = computed(() => {
        if (currStep.value === 1) {
            if (!wizardData.name.trim() || !wizardData.startDate) return false;
    
            if (wizardData.endDate && new Date(wizardData.endDate) < new Date(wizardData.startDate)) {
                return false;
            }
            return true;
        }
        if (currStep.value === 2) {
            return wizardData.customerCompany.trim() !== '' && wizardData.executorCompany.trim() !== '';
        }
        if (currStep.value === 3) {
            return wizardData.projectManagerId !== null && wizardData.projectManagerId > 0;
        }
        return true;
    });

    // STEP 3: MANAGERS
    const fetchManagers = async () => {
        try{
            const data = await EmployeesService.getManagers()
            managersList.value = data.map(emp => ({
                id: emp.id,
                fullName: `${emp.lastName} ${emp.firstName} ${emp.middleName || ''}`.trim()
            }))
        }
        catch (err){
            console.error('Failed to fetch managers:', err)
            globalError.value = 'Failed to load managers list.'
        }
    }

    const selectManager = (id: number, name: string) => {
        wizardData.projectManagerId = id
        wizardData.projectManagerName = name
    }

    // STEP 4: FINDING AND APPOINTING STAFF
    const searchEmployees = async (term: string) => {
        if (!term.trim()){
            searchResult.value = []
            return
        }
        try{
            isSearching.value = true
            const data = await EmployeesService.search(term)
            searchResult.value = data.map(emp => ({
                id: emp.id,
                fullName: `${emp.lastName} ${emp.firstName} ${emp.middleName || ''}`.trim()
            }))
        }
        catch (err){
            console.error('Employee search error:', err)
        }
        finally{
            isSearching.value = false
        }
    }

    // Debounce for search
    const handleSearchInput = () => {
        clearTimeout(debounceTimeout)
        debounceTimeout = setTimeout(() => {
            searchEmployees(searchQuery.value)
        }, 400)
    }

    const addEmployee = (emp: { id: number; fullName: string }) =>{
        if (!wizardData.employees.some(e => e.id === emp.id)) {
            wizardData.employees.push(emp)
        }
        searchQuery.value = ''
        searchResult.value = []
    }

    const removeEmployee = (id: number) => {
        wizardData.employees = wizardData.employees.filter(e => e.id !== id)
    }

    // STEP 5: FILES
    const handleFileChange = (e: Event) =>{
        const target = e.target as HTMLInputElement
        if (target.files){
            Array.from(target.files).forEach(file => {
                if (!wizardData.files.some(f => f.name === file.name && f.size === file.size)){
                    wizardData.files.push(file)
                }
            })
        }
    }

    const removeFile = (index: number) => {
        wizardData.files.splice(index, 1)
    }

    // Final shipment
    const submitProject = async () => {
        try{
            isLoading.value = true
            globalError.value = ''

            const payload ={
                name: wizardData.name,
                startDate: wizardData.startDate,
                endDate: wizardData.endDate || null,
                priority: Number(wizardData.priority),
                customerCompany: wizardData.customerCompany,
                executorCompany: wizardData.executorCompany,
                projectManagerId: wizardData.projectManagerId,
                employeeIds: wizardData.employees.map(e => e.id)
            }

            const result = await ProjectsService.create(payload)
            const createdProjectId = result.id

            if (wizardData.files.length > 0){
                for (const file of wizardData.files){
                    await ProjectsService.uploadDocument(createdProjectId, file)
                }
            }

            router.push({ name: 'dashboard' })
        }
        catch (err: any){
            console.error('Failed to create project:', err)
            globalError.value = err.response?.data?.message || 'Failed to create project. Please try again.'
        }
        finally{
            isLoading.value = false
        }
    }
    onMounted(() => {
        fetchManagers()
    })

    return {
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
    }
}