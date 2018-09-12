<template lang="pug">

  span
    v-icon(
      @click.stop=""
      small
      class="mr-2"
      @click="showDialog=true"
    ) visibility

    v-paper-dialog(
      :show="showDialog"
      :value="text"
      hide-title
    )
      
</template>

<script>
  import VPaperDialog from './VPaperDialog.vue'
  export default {
    props: {
      text: String
    },

    components: {
      VPaperDialog
    },

    data: () => ({
      showDialog: false
    })
  }
</script>
