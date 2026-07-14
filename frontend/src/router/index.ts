import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import LoginView from '@/views/LoginView.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/login',
      name: 'login',
      component: LoginView,
      meta: { requiresGuest: true }
    },
    {
      path: '/change-password',
      name: 'change-password',
      component: () => import('@/views/ChangePasswordView.vue'),
      meta: { requiresAuth: true }
    },
    {
      path: '/',
      name: 'dashboard',
      component: () => import('@/views/DashboardView.vue'),
      meta: { requiresAuth: true, requiresRealPassword: true }
    },
    {
      path: '/projects/create',
      name: 'create-project',
      component: () => import('@/views/CreateProjectView.vue'),
      meta: { 
        requiresAuth: true, 
        requiresRealPassword: true,
        allowedRoles: ['Director', 'ProjectManager']
      }
    },
    {
      path: '/employees/create',
      name: 'create-employee',
      component: () => import('@/views/EmployeesManagementView.vue'),
      meta: { requiresAuth: true, requiresRealPassword: true, allowedRoles: ['Director'] }
    },
    {
      path: '/projects/:id',
      name: 'project-details',
      component: () => import('@/views/ProjectDetailsView.vue'),
      meta: { requiresAuth: true, requiresRealPassword: true }
    }
  ],
})

router.beforeEach(async (to) => {

  const authStore = useAuthStore()

  if (authStore.isLoading){
    await authStore.checkAuth()
  }

  if(to.meta.requiresAuth && !authStore.isAuthenticated){
    return {name: 'login'}
  }

  if (to.meta.requiresGuest && authStore.isAuthenticated) {
    return { name: 'dashboard' }
  }

  if (authStore.isAuthenticated && authStore.mustChangePassword && to.name !== 'change-password') {
    alert('You must change your temporary password before proceeding!')
    return { name: 'change-password' }
  }

  if (authStore.isAuthenticated && !authStore.mustChangePassword && to.name === 'change-password') {
    return { name: 'dashboard' }
  }

  if (authStore.isLoading) {
    await authStore.checkAuth()
  }

  if (to.meta.allowedRoles){
    const allowed = to.meta.allowedRoles as string[]
    const userRole = authStore.user?.role || ''

    if (!allowed.includes(userRole)) {
      alert('Access is prohibited! You do not have enough rights.') 
      return { name: 'dashboard' }
    }
  }

  return true

})

export default router
