<template lang="pug">
  v-flex(xs12)
    v-select(
      v-model="selected"
      label="I use chips"
      chips
      :items="items"
      :value="values"
      :name="field.name"
      item-text="title"
      item-value="value"
      deletable-chips
      autocomplete
      multiple
      multi-line
      no-data-text="Opção não disponível"
    )

    v-select(
      :label="field.title"
      :items="items"
      :name="field.name"
      :value="values"
      item-text="title"
      item-value="value"
      v-model="selected"
      return-object
      autocomplete
      no-data-text="Opção não disponível"
    )
</template>

<script>
  export default {
    props: ['field'],

    data: () => ({
      selected: []
    }),

    computed: {
      items () {
        return this.field.value.items
      },

      values () {
        return this.selected.toString()
      }
    }
  }
</script>
