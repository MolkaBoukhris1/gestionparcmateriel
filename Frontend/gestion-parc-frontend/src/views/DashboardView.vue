<template>
  <div class="container-fluid py-4">
    <h2 class="mb-4">📊 Tableau de bord</h2>

    <div v-if="loading" class="text-center py-5">
      <div class="spinner-border text-primary" role="status"></div>
    </div>

    <div v-else>
      <!-- Cartes statistiques -->
      <div class="row mb-4">
        <div class="col-md-3 mb-3">
          <div class="card text-white bg-primary h-100">
            <div class="card-body">
              <h6 class="card-title">Total matériel</h6>
              <p class="display-6 mb-0">{{ dashboard.totalMateriel }}</p>
            </div>
          </div>
        </div>

        <div
          v-for="item in dashboard.repartitionParEtat"
          :key="item.etat"
          class="col-md-3 mb-3"
        >
          <div class="card h-100" :class="badgeClass(item.etat)">
            <div class="card-body">
              <h6 class="card-title">{{ item.etat }}</h6>
              <p class="display-6 mb-0">{{ item.count }}</p>
            </div>
          </div>
        </div>
      </div>

      <!-- Derniers mouvements -->
      <div class="card">
        <div class="card-header bg-dark text-white">
          Derniers mouvements
        </div>
        <div class="card-body p-0">
          <table class="table table-hover mb-0">
            <thead>
              <tr>
                <th>Date</th>
                <th>Matériel</th>
                <th>Type</th>
                <th>Employé</th>
                <th>Localisation</th>
              </tr>
            </thead>
            <tbody>
              <tr v-if="dashboard.derniersMouvements.length === 0">
                <td colspan="5" class="text-center text-muted py-3">
                  Aucun mouvement pour l'instant.
                </td>
              </tr>
              <tr
                v-for="mvt in dashboard.derniersMouvements"
                :key="mvt.id"
              >
                <td>{{ formatDate(mvt.date) }}</td>
                <td>{{ mvt.materiel }}</td>
                <td>{{ mvt.type }}</td>
                <td>{{ mvt.employe || '-' }}</td>
                <td>{{ mvt.localisation }}</td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import api from '../services/api'

const dashboard = ref({
  totalMateriel: 0,
  repartitionParEtat: [],
  derniersMouvements: []
})
const loading = ref(true)

const badgeClass = (etat) => {
  const map = {
    'En stock': 'bg-success text-white',
    'Affecté': 'bg-info text-white',
    'En maintenance': 'bg-warning',
    'Panne': 'bg-danger text-white',
    'HS': 'bg-secondary text-white',
    'Rebut': 'bg-dark text-white'
  }
  return map[etat] || 'bg-light'
}

const formatDate = (date) => {
  return new Date(date).toLocaleString('fr-FR')
}

onMounted(async () => {
  try {
    const response = await api.get('/Dashboard')
    dashboard.value = response.data
  } catch (error) {
    console.error('Erreur chargement dashboard', error)
  } finally {
    loading.value = false
  }
})
</script>