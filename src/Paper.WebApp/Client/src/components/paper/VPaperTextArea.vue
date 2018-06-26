<template lang="pug">
  v-text-field(
    :label="field.title"
    :value="field.value"
    :name="field.name"
    v-model="field.value"
    textarea
  )
</template>

<script>
  export default {
    props: ['field']
  }
</script>
