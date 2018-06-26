<template lang="pug">
  v-container(
    fluid
    fill-height
  )
    v-container(
      fluid 
      class="text-xs-center"
    )
      v-layout(
        flex 
        wrap
        row
        justify-space-between
      )
        v-flex(xs12)
          h1 {{ status }}

        v-flex(xs12)
          h2 {{ message }}
</template>

<script>
  export default {
    props: {
      error: Object
    },

    computed: {
      show () {
        return this.error && this.error.response
      },

      message () {
        if (this.show) {
          return this.$paper.errors.httpTranslate(this.error.response.status)
        }
        return 'Página de Erro'
      },

      status () {
        if (this.show) {
          return this.error.response.status
        }
        return 'ERRO'
      }
    }
  }
</script>