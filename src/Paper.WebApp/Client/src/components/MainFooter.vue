<template lang="pug">
    v-footer(
      color="primary"
    )
      v-flex(
        xs12
        py-3
        white--text
      ) 
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