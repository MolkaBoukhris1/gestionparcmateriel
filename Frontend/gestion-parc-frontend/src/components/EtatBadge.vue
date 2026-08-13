<template>
  <span class="badge" :class="badgeClass">{{ libelle }}</span>
</template>

<script setup>
import { computed } from 'vue'

const props = defineProps({ libelle: String })

const badgeClass = computed(() => {
  switch ((props.libelle || '').toLowerCase()) {
    case 'en stock':       return 'bg-success'
    case 'affecté':        return 'bg-primary'
    case 'en maintenance': return 'bg-warning text-dark'
    case 'panne':          return 'bg-danger'
    case 'hs':             return 'bg-danger'
    case 'rebut':          return 'bg-secondary'
    default:               return 'bg-light text-dark'
  }
})
</script>
