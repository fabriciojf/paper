<template lang="pug">

  span
    v-icon(
      @click.stop=""
      small
      class="mr-2"
      @click="showDialog"
    ) visibility

    v-paper-dialog(
      :value="text"
      :show.sync="dialogVisible"
      hide-title
    )
      
</template>

<script>
  import VPaperDialog from './VPaperDialog.vue'
  export default {
    props: ['text'],

    components: {
      VPaperDialog
    },

    data: () => ({
      dialogVisible: false
    }),

    methods: {
      showDialog () {
        this.dialogVisible = true
      }
    }
  }
</script>
