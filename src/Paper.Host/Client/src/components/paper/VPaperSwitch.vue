<template lang="pug">
  v-container(class="noPaddingLeft")
    v-switch(
      :label="field.title"
      :value="value"
      :name="field.name"
      v-model="field.value"
    )
</template>

<script>
  export default {
    props: ['field'],

    computed: {
      value () {
        return this.field.value !== undefined && this.field.value !== null ? this.field.value : false
      }
    }
  }
</script>

<style lang="sass">
  .noPaddingLeft
    padding-left: 0px
</style>
