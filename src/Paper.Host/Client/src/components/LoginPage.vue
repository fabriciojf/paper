<template lang="pug">
  v-container(
    fluid
  )
    v-flex(
      xs12
      sm6
      offset-sm3
    )

      v-card
        v-card-title(primary-title)
          h2 Login

        v-card-text
          v-form
            v-layout(column)

              v-flex
                v-text-field(
                  name="email"
                  label="Email"
                  id="email"
                  type="email"
                  v-model="data.body.username"
                  required
                )

              v-flex
                v-text-field(
                  name="password"
                  label="Senha"
                  id="password"
                  type="password"
                  v-model="data.body.password"
                  required
                )

              v-flex
                v-switch(
                  v-model="data.rememberMe"
                  label="Lembrar-me"
                )

              v-flex(
                class="text-xs-center"
                mt-5
              )
                v-btn(
                  color="primary"
                  @click.stop="login()"
                ) Entrar

                v-btn(
                  color="primary"
                  @click.stop="signup()"
                ) Quero me cadastrar

              div(
                v-show="error"
                style="color:red; word-wrap:break-word;"
              ) {{ error | json }}
</template>

<script>
  export default {
    data: () => ({
      data: {
        body: {
          username: 'admin',
          password: 'admin'
        },
        rememberMe: false
      },
      error: null
    }),

    created () {
      this.$paper.navigation.closeRightMenu()
    },

    methods: {
      login () {
        this.$paper.auth.login(this.data.body)
      }
    }
  }
</script>
