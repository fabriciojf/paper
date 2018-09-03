<template lang="pug">
    v-switch(
      :label="field.title"
      :name="field.name"
      v-model="field.value"
      class="ma-0 pa-0"
    )
</template>

<script>
  export default {
    props: ['field']
  }
</script>
