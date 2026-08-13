import { createRouter, createWebHistory } from 'vue-router'
import InsightsView    from '../views/InsightsView.vue'
import MaterielListe   from '../views/MaterielListe.vue'
import MaterielForm    from '../views/MaterielForm.vue'
import MaterielDetail  from '../views/MaterielDetail.vue'
import EmployeListe    from '../views/EmployeListe.vue'
import EmployeForm     from '../views/EmployeForm.vue'
import LocalisationListe from '../views/LocalisationListe.vue'
import DashboardView   from '../views/DashboardView.vue'
import LoginView       from '../views/LoginView.vue'

const routes = [
  { path: '/insights',                 component: InsightsView,     name: 'insights',         meta: { requiresAuth: true } },
  { path: '/',                        component: MaterielListe,    name: 'materiels',        meta: { requiresAuth: true } },
  { path: '/materiels/nouveau',        component: MaterielForm,     name: 'materiel-nouveau', meta: { requiresAuth: true } },
  { path: '/materiels/:id/modifier',   component: MaterielForm,     name: 'materiel-modifier', meta: { requiresAuth: true } },
  { path: '/materiels/:id',            component: MaterielDetail,   name: 'materiel-detail',  meta: { requiresAuth: true } },
  { path: '/employes',                 component: EmployeListe,     name: 'employes',         meta: { requiresAuth: true } },
  { path: '/employes/nouveau',         component: EmployeForm,      name: 'employe-nouveau',  meta: { requiresAuth: true } },
  { path: '/employes/:id/modifier',    component: EmployeForm,      name: 'employe-modifier', meta: { requiresAuth: true } },
  { path: '/localisations',            component: LocalisationListe, name: 'localisations',   meta: { requiresAuth: true } },
  { path: '/dashboard',                component: DashboardView,    name: 'dashboard',        meta: { requiresAuth: true } },
  { path: '/login',                    component: LoginView,        name: 'login' },
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

// ── Garde de navigation — bloque l'accès si non connecté ─────────────────────
router.beforeEach((to, from, next) => {
  const isLoggedIn = !!localStorage.getItem('token')

  if (to.meta.requiresAuth && !isLoggedIn) {
    next('/login')
  } else {
    next()
  }
})

export default router