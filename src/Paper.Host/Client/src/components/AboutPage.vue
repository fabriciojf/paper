<template lang="pug">
  v-container(
      fluid
    )
      v-flex(
        xs12
        sm8
        offset-sm2
      )
        v-card(
          class="elevation-3"
        )
          v-card-title(primary-title)
            h2 {{ $paper.blueprint.title }}

          v-card-text
            v-container(fluid)
              v-flex(
                xs12 
                sm12
              )
                v-list(two-line)
                  v-list-tile(
                    v-for="item in items" 
                    :key="item.key"
                  )
                    v-list-tile-content
                      v-list-tile-title
                        | {{ item.value }}
                      v-list-tile-sub-title
                        | {{ item.key }}
</template>

<script>
  export default {
    data: () => ({
      headers: []
    }),

    computed: {
      items () {
        var data = this.$paper.blueprint.projectInfo
        var items = []
        if (data) {
          var keys = Object.keys(data)
          keys.forEach(function (key) {
            items.push({
              key: key,
              value: data[key]
            })
          })
        }
        return items
      }
    },

    created () {
      this.$paper.navigation.setRightMenuVisible(false)
    }
  }
</script>
