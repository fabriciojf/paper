<template lang="pug">
    v-footer(
      color="primary"
      dark
      height="auto"
    )
      v-card(
        flat
        color="primary"
        dark
      )
        v-card-text(class="white--text")
          | KeepCoding &copy; 2018  v{{ version }}
          
</template>

<script>
  export default {
    computed: {
      version () {
        var pkg = require('../../package.json')
        return pkg.version
      }
    }
  }
</script>>