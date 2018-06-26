<template lang="pug">
  v-text-field(
    :label="field.title" 
    :value="field.value"
    :name="field.name"
    v-model="field.value"
    v-money="money"
    prefix="R$"
  )
</template>

<script>
  import {VMoney} from 'v-money'
  export default {
    props: ['field'],

    data: () => ({
      money: {
        decimal: ',',
        thousands: '.',
        precision: 2,
        masked: false
      }
    }),

    directives: {
      money: VMoney
    }
  }
</script>
