<template lang="pug">
  v-layout(
    row 
    wrap
  )
    v-flex(
      xs12
      sm6
    )
      v-menu(
        ref="menuDate"
        lazy
        :close-on-content-click="false"
        v-model="menuDate"
        transition="scale-transition"
        offset-y
        full-width
        :nudge-right="40"
        :return-value.sync="date"
      )
        v-text-field(
          :name="field.name"
          :label="field.title"
          :value="dateValue"
          slot="activator"
          v-model="dateFormatted"
          append-icon="event"
        )
        v-date-picker(
          v-model="date" 
          :first-day-of-week="1"
          locale="pt-br"
          no-title 
          scrollable
        )
          v-spacer
          v-btn(
            flat 
            color="primary" 
            @click="menuDate = false"
          ) Cancelar

          v-btn(
            flat 
            color="primary" 
            @click="save(date)"
          ) OK
</template>

<script>
  export default {
    props: ['field'],

    data: () => ({
      date: null,
      dateValue: null,
      dateFormatted: null,
      menuDate: false
    }),

    methods: {
      formatDate (date) {
        if (!date) {
          return null
        }

        const [year, month, day] = date.split('-')
        return `${day}/${month}/${year}`
      },

      parseDate (date) {
        if (!date) {
          return null
        }

        const [year, month, day] = date.split('-')
        return `${day}-${month}-${year}`
      },

      save (date) {
        this.dateFormatted = this.formatDate(date)
        this.dateValue = this.parseDate(date)
        this.menuDate = false
      }
    }
  }
</script>
