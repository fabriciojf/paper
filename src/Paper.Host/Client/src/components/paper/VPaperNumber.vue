<template lang="pug">
  v-text-field(
    :label="field.title" 
    :value="field.value"
    :name="field.name"
    v-model="field.value"
    type="number"
    step="any"
  )
</template>

<script>
  export default {
    data: () => ({
      number: ''
    }),

    props: ['field']
  }
</script>
