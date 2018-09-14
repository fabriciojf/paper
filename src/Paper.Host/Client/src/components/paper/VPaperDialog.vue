<template lang="pug">

  v-dialog(
    v-model="show"
    max-width="600px"
    scrollable 
  )
    v-card

      v-card-title(
        v-if="!hideTitle && title"
        primary-title
      ) {{ title }}

      v-divider(v-if="!hideTitle && title")

      v-card-text {{ value }}
      
      v-divider

      v-card-actions
        v-spacer
        v-btn(
          color="secondary" 
          flat 
          @click="close"
        ) Fechar
      
</template>

<script>
  export default {
    props: {
      value: String,
      title: String,
      hideTitle: {
        type: Boolean,
        default: false
      },
      show: {
        type: Boolean,
        default: false,
        required: true
      }
    },

    methods: {
      close () {
        this.$emit('update:show', false)
      }
    }
  }
</script>
