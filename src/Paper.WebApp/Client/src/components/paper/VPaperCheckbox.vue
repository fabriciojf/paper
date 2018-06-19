<template lang="pug">
  v-checkbox(
    color="primary"
    :name="field.name"
    :value="field.value"
    v-model="field.value"
    @click.stop="field.value = !field.value"
  )
    div(slot="label")
      | {{field.title}}
</template>

<script>
  export default {
    props: ['field']
  }
</script>
