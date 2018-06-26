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
          h1 404

        v-flex(xs12)
          h2 Página não encontrada!
          
        v-flex(
          xs12 
          class="mt-5"
          v-if="$paper.blueprint.hasPlannerPage()"
        )
          v-btn(
            color="primary" 
            dark 
            @click.stop="createRouter()"
          ) Projetar essa rota
</template>

<script>
  export default {
    props: {
      routerName: String
    },

    data () {
      return {
        dialog1: 'sandbox',
        dialog: false
      }
    },

    methods: {
      createRouter () {
        var planner = this.$paper.blueprint.getPlannerPage()
        this.$paper.requester.redirectToPage(planner)
      }
    }
  }
</script>