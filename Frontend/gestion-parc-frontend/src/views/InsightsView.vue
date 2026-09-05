<template>
  <div>
    <div class="d-flex justify-content-between align-items-center mb-4">
      <div>
        <h2 class="fw-bold mb-1">🧠 Analyses intelligentes</h2>
        <p class="text-muted mb-0 small">Alertes et recommandations générées automatiquement à partir de l'état du parc.</p>
      </div>
     <div class="d-flex gap-2">
        <button class="btn btn-outline-primary btn-sm" @click="charger" :disabled="loading">
          <span v-if="loading" class="spinner-border spinner-border-sm me-1"></span>
          🔄 Actualiser
        </button>
        <button class="btn btn-primary btn-sm" @click="genererAnalyseIA" :disabled="aiLoading">
          <span v-if="aiLoading" class="spinner-border spinner-border-sm me-1"></span>
          ✨ Générer une analyse IA
        </button>
      </div>
    </div>

    <!-- Bloc analyse IA -->
    <div v-if="aiSummary || aiLoading" class="card mb-4 border-0 ai-card">
      <div class="card-body">
        <h6 class="fw-bold mb-2 d-flex align-items-center gap-2">
          ✨ Analyse générée par IA
        </h6>
        <div v-if="aiLoading" class="text-muted small">
          <span class="spinner-border spinner-border-sm me-2"></span>
          Génération en cours...
        </div>
        <p v-else class="mb-0" style="white-space: pre-line">{{ aiSummary }}</p>
      </div>
    </div>

    <!-- Résumé rapide -->
    <div v-if="!loading" class="row g-2 mb-4">
      <div class="col-auto" v-for="stat in resume" :key="stat.type">
        <span class="badge px-3 py-2" :class="badgeResume(stat.type)">
          {{ stat.icone }} {{ stat.count }} {{ stat.label }}
        </span>
      </div>
    </div>

    <div v-if="loading" class="text-center py-5">
      <div class="spinner-border text-primary"></div>
    </div>

    <div v-else class="row g-3">
      <div v-for="(insight, i) in insights" :key="i" class="col-md-6">
        <div class="card h-100 insight-card" :class="'insight-' + insight.type">
          <div class="card-body d-flex gap-3 align-items-start">
            <div class="insight-icon">{{ insight.icone }}</div>
            <div class="flex-grow-1">
              <div class="d-flex justify-content-between align-items-start">
                <h6 class="fw-bold mb-1">{{ insight.titre }}</h6>
                <span class="badge rounded-pill" :class="badgeType(insight.type)">
                  {{ labelType(insight.type) }}
                </span>
              </div>
              <p class="mb-0 small text-muted">{{ insight.message }}</p>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import api from '../services/api.js'

const insights = ref([])
const loading = ref(true)
const aiSummary = ref('')
const aiLoading = ref(false)

const resume = computed(() => {
  const counts = {}
  insights.value.forEach(i => {
    counts[i.type] = (counts[i.type] || 0) + 1
  })
  const labels = {
    danger:    { icone: '🔴', label: 'critique(s)' },
    warning:   { icone: '⚠️', label: 'à surveiller' },
    info:      { icone: '📌', label: 'information(s)' },
    secondary: { icone: '📦', label: 'stock dormant' },
    success:   { icone: '✅', label: 'ok' }
  }
  return Object.entries(counts).map(([type, count]) => ({
    type, count, icone: labels[type]?.icone || '•', label: labels[type]?.label || type
  }))
})

function badgeResume(type) {
  const map = {
    danger: 'bg-danger text-white', warning: 'bg-warning', info: 'bg-info text-white',
    secondary: 'bg-secondary text-white', success: 'bg-success text-white'
  }
  return map[type] || 'bg-light'
}

function badgeType(type) {
  const map = {
    danger: 'bg-danger', warning: 'bg-warning text-dark', info: 'bg-info text-white',
    secondary: 'bg-secondary', success: 'bg-success'
  }
  return map[type] || 'bg-light text-dark'
}

function labelType(type) {
  const map = { danger: 'Critique', warning: 'Attention', info: 'Info', secondary: 'Stock', success: 'OK' }
  return map[type] || type
}

async function charger() {
  loading.value = true
  try {
    const res = await api.get('/Insights')
    insights.value = res.data
  } catch (error) {
    console.error('Erreur chargement insights', error)
  } finally {
    loading.value = false
  }
}
async function genererAnalyseIA() {
  aiLoading.value = true
  try {
    const res = await api.get('/Insights/ai-summary')
    aiSummary.value = res.data.analyse
  } catch (error) {
    aiSummary.value = "Erreur lors de la génération de l'analyse."
  } finally {
    aiLoading.value = false
  }
}
onMounted(charger)
</script>

<style scoped>
.ai-card {
  background: linear-gradient(135deg, #eef4ff 0%, #f3ecff 100%);
  border-left: 4px solid #7c3aed !important;
}
.insight-card {
  border-left: 4px solid var(--border);
  transition: transform 0.2s ease;
}
.insight-card:hover {
  transform: translateX(2px);
}
.insight-icon {
  font-size: 1.6rem;
  line-height: 1;
}
.insight-danger    { border-left-color: #dc2626; }
.insight-warning   { border-left-color: #d97706; }
.insight-info      { border-left-color: #0891b2; }
.insight-secondary { border-left-color: #64748b; }
.insight-success   { border-left-color: #059669; }
</style>