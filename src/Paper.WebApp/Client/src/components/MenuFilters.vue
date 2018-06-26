<template lang="pug">
  v-navigation-drawer(
    fixed
    left
    v-model="$store.state.filters.openedFiltersMenu"
    clipped
    app
    v-if="show"
    class="grey lighten-4"
    style="max-height: calc(100% - 60px)"
    :width="navigationSize"
  )
    v-subheader APLICAR FILTROS

    v-container(
      fluid
      class="nobordertop"
    )
      v-layout(
        flex
        row
      )
        v-flex(xs12)
          paper-form(
            :action="$paper.filters.entity"
          )

</template>

<script>
  import FormsMixin from '../mixins/FormsMixin.js'
  import PaperForm from './paper/VPaperForm.vue'
  export default {
    beforeRouteUpdate (to, from, next) {
      next()
    },

    mixins: [
      FormsMixin
    ],

    methods: {
      openActionPage (action) {
        var params = this.$route.params.path
        this.$paper.requester.redirectToForm(params, action.name)
      },

      openAboutPage () {
        this.$router.push({ name: 'about' })
      }
    },

    components: {
      PaperForm
    },

    computed: {
      show () {
        var show = this.$paper.filters.hasFilters() &&
                   this.$store.state.filters.openedFiltersMenu &&
                   !this.$paper.isFormPage(this.$route.name) &&
                   !this.$paper.state.selection
        return show
      },

      navigationSize () {
        return 300
      }
    }
  }
</script>

<style lang="sass">
  .nobordertop
    padding-top: 0px

  .noborderbottom  
    padding-bottom: 0px

  .headerDivider
     border-left: 1px solid #38546d 
     border-right: 1px solid #16222c
     height: 80px
     position: absolute
     right: 249px
     top: 10px
</style>
