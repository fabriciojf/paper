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
        lazy
        :close-on-content-click="false"
        v-model="menu"
        transition="scale-transition"
        offset-y
        full-width
        :nudge-right="40"
      )
        v-text-field(
          :name="field.name"
          :label="field.title"
          :value="time"
          slot="activator"
          v-model="time"
          append-icon="access_time"
          readonly
        )
        v-time-picker(
          v-model="time" 
          format="24hr"
        )
          template(slot-scope="{ save, cancel }")
            v-card-actions
              v-spacer
              v-btn(
                flat 
                color="primary" 
                @click="cancel"
              ) Cancelar

              v-btn(
                flat 
                color="primary" 
                @click="save"
              ) OK
</template>

<script>
  export default {
    props: ['field'],

    data: () => ({
      time: null,
      menu: false,
      modal: false
    })
  }
</script>
