<template lang="pug">
  v-container(
    fluid
  )
    v-flex(
      xs12
      sm8
      offset-sm2
    )
      v-card(
        class="elevation-3"
      )
        v-card-title(
          primary-title
          class="form-title"
        )
          h2 {{ action ? action.title : '' }}

        v-card-text
          paper-form(:action="action")
</template>

<script>
  import PaperForm from './paper/VPaperForm.vue'
  export default {
    components: {
      PaperForm
    },

    computed: {
      action () {
        var actionName = this.$route.query.action
        var action = this.$paper.actions.getAction(actionName)
        return action
      }
    },

    beforeRouteUpdate (to, from, next) {
      if (to.params.length > 0) {
        var path = '/' + to.params.path.join('/')
        this.$paper.setEntityPath(path)
      }
      next()
    },

    created () {
      var path = '/' + this.$route.params.path
      if (this.$route.params.path instanceof Array) {
        path = '/' + this.$route.params.path.join('/')
      }
      this.$paper.setEntityPath(path)
      this.$paper.page.load()
    }

  }
</script>

<style lang="sass">
  .form-title
    padding-bottom: 0px
</style>
