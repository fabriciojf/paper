<template lang="pug">
  label(class="subheading") {{ field.value }}
</template>

<script>
  export default {
    props: ['field']
  }
</script>
