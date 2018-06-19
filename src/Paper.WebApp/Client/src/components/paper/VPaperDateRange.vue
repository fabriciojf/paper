<template lang="pug">
    v-layout(
      row
      wrap
    )
      v-flex(
        xs11
        sm6
      )
        v-paper-date(:field="field")

      v-spacer

      v-flex(
        xs11
        sm6
      )
        v-paper-date(:field="field")
      
</template>

<script>
  import VPaperDate from './VPaperDate.vue'
  export default {
    components: {
      VPaperDate
    },

    props: ['field']
  }
</script>
