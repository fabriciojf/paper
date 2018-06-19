<template lang="pug">
  div
    v-btn {{ field.value }}
</template>

<script>
  export default {
    props: ['field']
  }
</script>
