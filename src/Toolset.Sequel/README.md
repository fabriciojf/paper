Sequel
======

API de simplificação de consultas SQLs embarcadas no código C#.


Visão Geral
-----------

Sequel é uma API para escrita e execução de consultas SQL diretamente
no código C#.

A API oferece:

-   Gerenciamento de conexão multi-thread.
-   Gerenciamento de pool de conexões por base de dados e contexto.
-   Template de construção de SQLs.
-   Parametrização de consultas.
-   Conversão e formatação do resultado de consultas.


1. Sintaxe Geral
----------------

A API do Sequel é composta de diversos métodos de extensão em cima do
objeto Sql do Sequel.

As instruções SQL são escritas diretamente em strings e convertidas
para o objeto Sql, que então recebe a aplicação dos métodos de extensão.

As chamadas da API devem ser aninhadas em escopos de conexão do Sequel,
o SequelScope, ou uma de suas extensões.

Todas as instâncias de SequelScope devem, obrigatoriamente, ser encapsuladas
em blocos using.

Exemplo:

    using (var scope = new SequelScope("conexao"))
    {
      var usuarios =
        @"select nome
            from usuario
           where ativo = @ativo"
          .AsSql()
          .Set("ativo", true)
          .SelectArray();

      foreach (var usuario in usuarions)
      {
        ...
      }
    }


2. Uso Geral da API
-------------------

A sessão seguinte apresenta um passo a passo de uso da API do Sequel
para atender a maior partes das situações.

### 2.1.  Escopos do Sequel

> ☛ Aninhe instruções do Sequel em blocos SequelScope.

O primeiro escopo aninhado deve referenciar uma conexão, nome de conexão ou
string de conexão. Quando uma conexão é aberta por um escopo o próprio
escopo se encarrega de fechá-la.

Usando uma conexão existente:

    var dbConnection = ...
    using (var scope = new SequelScope(dbConnection))
    {
      ...instrucoes...
    }
    // A conexão dbConnection continua existindo e aberta...

Usando uma string de conexão:

    var strConnection = "Data Source=server;Catalog=exemplo;User ID=sa;Password=123";
    using (var scope = new SequelScope(strConnection))
    {
      ...instrucoes...
      // A conexão é criada e fechado dentro do bloco
    }

Usando um nome de configuração

    // Deve haver uma string de conexão no App.config com o nome de "exemplo".
    using (var scope = new SequelScope("exemplo"))
    {
      ...instrucoes...
      // A conexão é criada e fechado dentro do bloco
    }

Um escopo aninhado pode omitir a configuração da conexão para compartilhar a conexão
do pai.

    // O primeiro escopo cria a conexão
    using (var scope = new SequelScope("exemplo"))
    {
      
      // O escopo aninha pode omitir a conexão para compartilhar a conexão do pai
      using (var scope = new SequelScope())
      {
        ...instrucoes...
      }

    }

O exemplo acima é ilustrativo mas não tem uso prático.
O uso real de escopo aninhado é entre chamadas de métodos.

Um método pode se manter alheio à conexão que será de fato usada enquanto exige
a abertura de um escopo do Sequel pela método invocador:

    void ApagarContas(Conta[] contas)
    {
      // abrindo uma conexao real
      using (var scope = new SequelScope("exemplo"))
      {
        foreach (var conta in contas)
        {

          // chamando um método que também utiliza um escopo do Sequel
          // o escopo será aninhado ao escopo atual e irá compartilhar sua conexão
          ApagarUsuario(conta.Login)

          ContasAtivas.Remove(conta);
        }
      }
    }

    void ApagarUsuario(string login)
    {
      // não importa qual a conexão, desde que tenha sido aberta previamente
      using (var scope = new SequelScope())
      {
        "delete from usuario where login = @login"
          .AsSql()
          .Set("login", login)
          .Execute();
      }
    }


### 2.2.  Convenção do Sequel

> ☛ Construa uma string na convenção do Sequel e a converta para o objeto Sql.

Convenção do Sequel:

-   Preceda as strings de SQL com `@`.
-   Escreva a SQL em várias linhas.
-   Indente a SQL.
-   Termine a string com uma aspa exatamente no fim da última instrução.
-   Converta a string para SQL usando a função `AsSql()` na mesma declaração da SQL.
-   Indente a função `AsSql()` dois espaços à frente do `@`.
-   Coloque cada chamada de API, como `Set()`, etc, em uma nova linha.

Exemplo:

    @"select *
        from usuario
       inner join grupo
               on grupo.id = usuario.grupo_id
       where ativo = @ativo"
      .AsSql()             ^
      .Set("ativo", true)  |
      .Select();           '- a aspa exatamente à frente da última instrução
    ^^    ^                
    |     '- uma única chamada de API por linha
    |
    '- indentado dois espaços à frente do `@`

Toda a instrução pode ser escrita em uma linha se couber dentro de 80 colunas:

    "delete from usuario".AsSql().Execute();

Se a SQL couber em 80 colunas mas as chamadas de API não, siga as instruções da
convenção para chamada de API:

    "delete from usuario where black_list = 1 and ativo = @ativo"
      .AsSql()
      .Set("ativo", false)
      .Select();
    ^^    ^                
    |     '- uma única chamada de API por linha
    |
    '- indentado dois espaços à frente do `@`

>   Nota:
>   
>   As funções `Beautify()` requerem uma string escrita exatamente nesta convenção
>   para corretamente indentar a SQL.


### 2.3.  Formatação Posicional e Nomeada

> ☛ Formate a SQL com parâmetros posicionais e nomeados.

A formatação de SQL permite a construção de SQLs dinâmicas.
O executor do Sequel aplica a formatação para construção da SQL definitiva antes
da sua execução.

A formatação posicional faz uso do String.Format do DotNet e suporta todas as suas
capacidades, como formatação de números e datas.

    var sql =
      @"select {1}, {2}
          from {0}"
        .AsSql()
        .Format("usuarios", "id", "nome")
        .ToString();
    Debug.WriteLine(sql);
    // Produz a saída:
    //   "select id, nome from usuarios"

A formatação nomeada faz uso da sintaxe especial "@{}" para substituir porções do
texto.

    var sql =
      @"select @{campo1}, @{campo2}
          from @{tabela}"
        .AsSql()
        .Set("tabela", "usuarios")
        .Set("campo1", "id")
        .Set("campo2", "nome")
        .ToString();
    Debug.WriteLine(sql);
    // Produz a saída:
    //   "select id, nome from usuarios"

>   Nota:
>   
>   Quando a manipulação da SQL para formatação for intensa e o desempenho se tornar uma
>   questão importante, prefira usar a formatação posicional, que faz uso de String.Format,
>   que é uma instrução otimizada do DotNet.


### 2.4.  Atribuição de Parâmetros

> ☛ Atribua valor aos parâmetros.

Os parâmetros são atribuídos pela função `Set()`, a mesma utilizada na definção de valores
da formatação nomeada.

Os parâmetros podem ser atribuídos um a um por chamadas à função `Set()` ou de uma só vez
pelas suas sobrecargas.

    var logins =
      @"select login
          from usuarios
         where ativo = @ativo
           and grupo = @grupo"
        .AsSql()
        .Set("ativo", ativo)
        .Set("grupo", "grupo")
        .SelectArray();

    // Embora a sintaxe acima seja válida, esta sintaxe é mais otimizada, porque
    // atribui os parâmetros de uma só vez:
    
    var logins =
      @"select login
          from usuarios
         where ativo = @ativo
           and grupo = @grupo"
        .AsSql()
        .Set(
          "ativo", ativo,
          "grupo", "grupo"
        )
        .SelectArray();

Para parametrização de consultas pelo operador `in` do SQL faça uso de formatação
nomeada. O Sequel se encarrega de encapsular strings e datas entre apóstrofos.

    var nomes = new{} { "Fulano", "Beltrano" };
    var logins =
      @"select login
          from usuarios
         where nome in ( @{nomes} )"
        .AsSql()
        .Set("nomes", nomes)
        .SelectArray();


### 2.5.  Execute SQLs e Consulte Resultados

> ☛ Resolva a SQL executando ou consultando seu resultado.

**Execuções:**

Execução deve ser feita pelo método `Execute()`.

    @"delete from usuario
       where ativo = 0
         and grupo = @grupo"
      .AsSql()
      .Set("grupo", grupo)
      .Execute();

**Consultas:**

Consultas se beneficiam das várias sobrecargas do método `Select()`.

`Select`
:   Obtém um resultset para os registros selecionados.
    
        using (var scope = new SequelScope())
        {
          var reader =
            "select * from usuarios"
              .AsSql()
              .Select();
              
          using (reader)
          {
            while (reader.Read())
            {
              Debug.WriteLine(reader.Get("id"));
              Debug.WriteLine(reader.Get("nome"));
            }
          }
        }

`Select<T>`
:   Obtém um resultset para a primeira coluna do resultado
    com valores convertidos para o tipo indicado.
    
        using (var scope = new SequelScope())
        {
          var reader =
            "select login from usuarios"
              .AsSql()
              .Select<string>();
              
          using (reader)
          {
            while (reader.Read())
            {
              string login = reader.Current;
              Debug.WriteLine(login);
            }
          }
        }

`SelectOne<T>` 
:   Similar ao `Select<T>`, porém, retorna apenas o primeiro registro.
    Se um registro não existir o valor padrão do tipo é retornado.
    
        using (var scope = new SequelScope())
        {
          var login =
            "select login from usuarios"
              .AsSql()
              .SelectOne<string>();
              
          Debug.WriteLine(login);
        }

`SelectGraph<T>`
:   Obtém um objeto determinado montado a partir dos campos do
    registro pela correspondência dos nomes. Portanto, basta que a
    consulta SQL retorne campos com os mesmos nomes dos campos no
    objeto.
    
        using (var scope = new SequelScope())
        {
          var reader =
            "select * from usuarios"
              .AsSql()
              .SelectGraph<Usuario>();
              
          using (reader)
          {
            while (reader.Read())
            {
              Usuario usuario = reader.Current;
              Debug.WriteLine(usuario.Login);
            }
          }
        }

`SelectOneGraph<T>`
:   Simimlar ao `SelectGraph<T>`, porém, retorna apenas um registro.
    Se um registro não existir nulo é retornado.

        using (var scope = new SequelScope())
        {
          var usuario =
            "select * from usuarios"
              .AsSql()
              .SelectOneGraph<Usuario>();
              
          Debug.WriteLine(usuario.Login);
        }

`SelectGraph<T1, T2, ...>`
:   Obtém uma tupla com os valores dos campos convertidos para os
    tipos indicados. Podem ser indicados de dois a sete tipos.
    A quantidade de campos considerados corresponde à quantidade de
    tipos indicados.
    
    Em tuplas os campos são acessados pelas propriedades ItemX,
    sendo X o índice do campo.

        using (var scope = new SequelScope())
        {
          var reader =
            "select id, login from usuarios"
              .AsSql()
              .Select<int, string>();
              
          using (reader)
          {
            while (reader.Read())
            {
              var id = reader.Item1;
              var login = reader.Item2;
              Debug.WriteLine(id);
              Debug.WriteLine(login);
            }
          }
        }

`SelectOneGraph<T1, T2, ...>`
:   Simimlar ao `SelectGraph<T1, T2, ...>`, porém, retorna apenas um
    registro.
    Se um registro não existir nulo é retornado.

        using (var scope = new SequelScope())
        {
          var tupla =
            "select id, login from usuarios"
              .AsSql()
              .SelectOne<int, string>();
              
          var id = reader.Item1;
          var login = reader.Item2;
          Debug.WriteLine(id);
          Debug.WriteLine(login);
        }

`SelectArray<T>`
:   Obtém um vetor contendo a primeira coluna com os valores
    convertidos para o tipo indicado.

        using (var scope = new SequelScope())
        {
          var nomes =
            "select nome from usuarios"
              .AsSql()
              .SelectArray<string>();
              
          foreach (var nome in nomes)
          {
            Debug.WriteLine(nome);
          }
        }

`Select<T>` com função de conversão
:   Obtém a coleção de registros convertidos para o tipo indicado
    por uma função personalizada.

        using (var scope = new SequelScope())
        {
          var reader =
            "select * from usuarios"
              .AsSql()
              .Select(x => new[] { x["id"], x["login"] });
          
          using (reader)
          {
            while (reader.Read())
            {
              object[] vetor = reader.Current;
              var id = vetor[0];
              var login = vetor[1];
              Debug.WriteLine(id);
              Debug.WriteLine(login);
            }
          }
        }

    Um uso comum para a função personalizada é a conversão do
    registro para um tipo anônimo, aproveitando os métodos Get
    sobrecarregados do objeto Record do Sequel:
    
        using (var scope = new SequelScope())
        {
          var reader =
            "select * from usuarios"
              .AsSql()
              .Select(x => new {
                 Id = x.Get<int>("id"),
                 Login = x.Get<string>("login")
              });
          
          using (reader)
          {
            while (reader.Read())
            {
              var id = reader.Current.Id;
              var login = reader.Current.Login;
              Debug.WriteLine(id);
              Debug.WriteLine(login);
            }
          }
        }

`SelectOne<T>` com função de conversão
:   Simimlar ao `SelectOne<T>` com função de conversão, porém,
    retorna apenas um registro.
    Se um registro não existir nulo é retornado.

        using (var scope = new SequelScope())
        {
          var usuario =
            "select * from usuarios"
              .AsSql()
              .SelectOne(x => new {
                 Id = x.Get<int>("id"),
                 Login = x.Get<string>("login")
              });
          
          var id = reader.Current.Id;
          var login = reader.Current.Login;
          Debug.WriteLine(id);
          Debug.WriteLine(login);
        }
        
**Métodos `Try*()`:**

Cada método de execução e consulta possui uma variação precedida por
`Try`.

Este tipo de método não produz exceção.

-   Os métodos de consulta retornam o valor padrão esperado em caso de falha.
-   Os métodos de execução saem silenciosamente em caso de falha.

**IResult:**
    
As consultas que retornam instâncias de IResult permitem a varredura de
resultados em cima de instâncias de DataReader. Os registros neste caso
são obtidos sob demanda.

As instâncias de IResult devem ser fechadas depois do uso,
preferencialmente em blocos using.

    var reader = "select * from usuario".AsSql().Select();
    using (reader)
    {
      while (reader.Read())
      {
        ...
      }
    }

### 2.6.  Transacione as Instruções SQL.
 
> ☛ Aninhe instruções SQL em blocos transacionais com o método
> `BeginTransacionScope()`

O Sequel possui um objeto de transação especial que simplifica a criação
de escopos transacionais: `SequelScope.BeginTransacionScope()`

BeginTransacionScope inicia um escopo para execução de um bloco de
instruções SQL. Ao final do processamento, antes do fechamento do bloco,
o método `Complete()` deve ser invocado.

No fechamento do bloco, se `Complete()` tiver sido invocado com sucesso
a transação é confirmada com `commit`, caso contrário, a transação é
cancelada com `rollback`.

Por convenção é recomendado chamar de `tx` a variável que contém a
transação dentro do bloco.

    using (var scope = new SequelScope("conexao"))
    using (var tx = scope.BeginTransactionScope())
    {
      "delete * from usuario".AsSql().Execute();
      "delete * from grupo_usuario".AsSql().Execute();
      
      tx.Complete(); // Se complete não for chamado a transação será cancelada.
    }

>   Nota:
>   
>   Como ilustrado no exemplo acima, quando todo o escopo do SequelScope
>   é transacionado é recomendado omitir as chaves do primeiro bloco
>   using.
>
>   Assim, em vez de:
>
>       using (var scope = new SequelScope("conexao"))
>       {
>         using (var tx = scope.CreateTransactionScope())
>         {
>           ...instrucoes...
>         }
>       }
>
>   Temos apenas:
>
>       using (var scope = new SequelScope("conexao"))
>       using (var tx = scope.CreateTransactionScope())
>       {
>         ...instrucoes...
>       }

### 2.7.  Indentação da SQL

Na criação de procedures, views e funções é esperado, geralmente, que o
texto SQL seja indentado, facilitando a leitura do código
posteriormente.

No Sequel, a indentação usada no código C# pode ser replicada na base
de dados com o uso da instrução `Beautify()`.

Beautify espera que a convenção do Sequel para escrita de instruções
SQL tenha sido adotada. Veja mais detalhes no início deste artigo.

Exemplo:

    @"create view vw_usuarios
      as
      select *
        from usuario
       inner join grupo_usuario
               on grupo_usuario.id = usuario.grupo_id"
      .AsSql()
      .Beautify()
      .Execute();
      
    // o corpo da view será:
    //
    // create view vw_usuarios
    // as
    // select *
    //   from usuario
    //  inner join grupo_usuario
    //          on grupo_usuario.id = usuario.grupo_id"
    // 


## 3. Uso Avançado

Além de oferecer a API de extensão para construção e execução de SQLs o
Sequel também administra conexões em ambientes multi-thread e pool de
conexões por base de dados.

### 3.1. Escopo Multi-Thread

O SequelScope isola conexões por thread, fazendo com que cada thread
abra sua própria conexão e compartilha conexão entre os métodos
invocados por ela enquanto o escopo inicial estiver aberto.

No exemplo abaixo duas threads são lançadas e cada uma recebe sua
própria conexão.

O método `ApagarUsuario` é usado pelas duas threads e é alheio á conexão
real em uso. Durante a execução de `ApagarUsuario` o Sequel seleciona a
conexão aninha no escopo da thread que a invocou.


    static void Main()
    {
      var primeira = Task.Factory.StartNew(() =>
      {
        
        using (var scope = new SequelScope("conexao")
        {
          var login = "fulano@host.com";
          ApagarUsuario(login);
        }
        
      });
      
      var segunda = Task.Factory.StartNew(() =>
      {
        
        using (var scope = new SequelScope("conexao")
        {
          var login = "beltrano@host.com";
          ApagarUsuario(login);
        }
        
      });
      
      Task.WaitAll(primeira, segunda);
    }

    void ApagarUsuario(string login)
    {
      // não importa qual a conexão, desde que tenha sido aberta previamente
      using (var scope = new SequelScope())
      {
        "delete from usuario where login = @login"
          .AsSql()
          .Set("login", login)
          .Execute();
      }
    }
    
O SequelScope, permite a abertura de uma nova conexão para cada thread,
sem limitar a quantidade máxima de conexões abertas.

Este cenário é ideal para ambientes multi-usuário, onde cada usuário
é isolado em sua própria thread e recebe o direito de se conectar com
a base de dados.

### 3.2. Escopo Compartilhado

Os escopos compartilhados, `SequelScope.Named` e `SequelScope.Shared`,
ainda permitem a abertura ilimitada de conexões para configurações
diferentes mas limitam o número de conexões abertas para uma mesma
configuração.

Uma configuração identifica uma base de dados específica, por tanto,
multiplas conexões anda são permitidas para bases diferentes mas são
limitadas para uma mesma base de dados.

A configuração é identificada pelo nome do escopo e pela configuração de
conexão.

A configuração de conexão pode ser a string de conexão ou o nome da
string de conexão na base de dados.

Escopos:

SequelScope.Shared
:   Este escopo permite a criação do escopo compartilhado padrão.

    Escopos compartilhados padrão compartilham o limite de conexão entre si
    mas não sofrem limitação pelos demais escopos.

        using (var scope = new SequelScope.Shared("conexao")
        {
          ...instrucoes...
        }

SequelScope.Named
:   Este escopo permite a criação do escopo nomeado.

    Escopos com o mesmo nome compartilham o limite de conexão entre si mas
    não sofrem limitação pelos demais escopos.

        using (var scope = new SequelScope.Named("escopo", "conexao")
        {
          ...instrucoes...
        }

Tamanho do Pool de Conexões:

Por padrão os pools de conexão tem tamanho ilimitado. A limitação dos
pools pode ser feita globalmente ou por escopo, diretamente no objeto
SequelSettings ou nas configurações do sistema.

App.config
:   A configuração pode ser feita na seção `<appSettings>` do arquivo 
    App.config do sistema, pela adição das seguintes chaves:

    Sequel.ConnectionPoolSize
    :   Esta chave  governa o tamanho padrão dos pools.
        Qualquer pool não configurado terá o tamanho definido por esta
        chave.
    
        O valor zero define um pool de tamanho ilimitado.
    
        Exemplo:
        
            // App.config
            
            <configuration>
              <appSettings>
                <add key="Sequel.ConnectionPoolSize" value="3" />
              </addSettings>
            </configuration>
            
            // Exemplo.cs
            
            class Program()
            {
              void Main()
              {
              
                // 10 threads estão sendo lançadas, mas apenas três
                // poderão se conectar por vez. As demais threads
                // ficarão numa fila aguardando a liberação de uma
                // conexão.
                for (int i = 0; i < 10; i++)
                {
                  Task.Factory.StartNew(() =>
                  {
              
                    using (var scope = new SequelScope.Shared("conexao")
                    {
                      ...instrucoes...
                    }

                  });
                }
                
                // Esta thread receberá sua conexão normalmente porque
                // tem seu escopo criado com SequelScope(), que não
                // participa do compartilhamento de escopo.
                Task.Factory.StartNew(() =>
                {
            
                  using (var scope = new SequelScope("conexao")
                  {
                    ...instrucoes...
                  }

                });
                
              }
              Thread.Sleep(5000);
            }
            
    Sequel.ConnectionPoolSize.*
    :   Chaves com este padrão governam o tamanho do pool de escopos
        específicos.
        
        O curinga `*` representa o nome do escopo, sensível a caso.

        O valor zero define um pool de tamanho ilimitado.
    
        Exemplo:
        
            // App.config
            
            <configuration>
              <appSettings>
                <add key="Sequel.ConnectionPoolSize.exemplo" value="3" />
              </addSettings>
            </configuration>
            
            // Exemplo.cs
            
            class Program()
            {
              void Main()
              {
              
                // 10 threads estão sendo lançadas, mas apenas três
                // poderão se conectar por vez. As demais threads
                // ficarão numa fila aguardando a liberação de uma
                // conexão.
                for (int i = 0; i < 10; i++)
                {
                  Task.Factory.StartNew(() =>
                  {
              
                    using (var scope = new SequelScope.Named("exemplo", "conexao")
                    {
                      ...instrucoes...
                    }

                  });
                }
                
                // Esta thread receberá sua conexão normalmente porque
                // tem um nome de escopo diferente daquele utilizado
                // mais acima.
                Task.Factory.StartNew(() =>
                {
            
                  using (var scope = new SequelScope.Named("sandbox", "conexao")
                  {
                    ...instrucoes...
                  }

                });
                
                // Esta thread receberá sua conexão normalmente porque
                // tem seu escopo criado com SequelScope(), que não
                // participa do compartilhamento de escopo.
                Task.Factory.StartNew(() =>
                {
            
                  using (var scope = new SequelScope("conexao")
                  {
                    ...instrucoes...
                  }

                });
                
              }
              Thread.Sleep(5000);
            }

Classe SequelSettings
:   A configuração pode ser feita na propriedade
    SequelSettings.PoolSize, pela definição de um limite padrão ou
    limite por escopo.
    
    SequelSettings.PoolSize.DefaultValue
    :   Esta propriedade governa o tamanho padrão dos pools.
        Qualquer pool não configurado terá o tamanho definido por esta
        propriedade.

        O valor zero define um pool de tamanho ilimitado.
        
        O valor padrão desta propriedade é lido do arquivo de 
        configuraçãoconforme apresentado mais acima.
    
        Exemplo:
        
            SequelSettings.PoolSize.DefaultValue = 3;
        
            // 10 threads estão sendo lançadas, mas apenas três
            // poderão se conectar por vez. As demais threads
            // ficarão numa fila aguardando a liberação de uma
            // conexão.
            for (int i = 0; i < 10; i++)
            {
              Task.Factory.StartNew(() =>
              {
          
                using (var scope = new SequelScope.Shared("conexao")
                {
                  ...instrucoes...
                }

              });
            }
            
            // Esta thread receberá sua conexão normalmente porque
            // tem seu escopo criado com SequelScope(), que não
            // participa do compartilhamento de escopo.
            Task.Factory.StartNew(() =>
            {
        
              using (var scope = new SequelScope("conexao")
              {
                ...instrucoes...
              }

            });
            
    SequelSettings.PoolSize[*]
    :   Esta propriedade governa o tamanho de um pool de um escopo
        nomeado. O asterisco representa o nome do escopo.

        O valor zero define um pool de tamanho ilimitado.
        
        O valor padrão desta propriedade é lido do arquivo de 
        configuração conforme apresentado mais acima.
    
        Exemplo:
        
            SequelSettings.PoolSize["exemplo"] = 3;
        
            // 10 threads estão sendo lançadas, mas apenas três
            // poderão se conectar por vez. As demais threads
            // ficarão numa fila aguardando a liberação de uma
            // conexão.
            for (int i = 0; i < 10; i++)
            {
              Task.Factory.StartNew(() =>
              {
          
                using (var scope = new SequelScope.Named("exemplo", "conexao")
                {
                  ...instrucoes...
                }

              });
            }
            
            // Esta thread receberá sua conexão normalmente porque
            // tem um nome de escopo diferente daquele utilizado
            // mais acima.
            Task.Factory.StartNew(() =>
            {
        
              using (var scope = new SequelScope.Named("sandbox", "conexao")
              {
                ...instrucoes...
              }

            });
            
            // Esta thread receberá sua conexão normalmente porque
            // tem seu escopo criado com SequelScope(), que não
            // participa do compartilhamento de escopo.
            Task.Factory.StartNew(() =>
            {
        
              using (var scope = new SequelScope("conexao")
              {
                ...instrucoes...
              }

            });


### 3.3. Escopo de Base de Dados

O Sequel tem um escopo adicional para aninhar instruções de uma base de
dados.

-   No início do escopo o Sequel seleciona a base de dados indicada.
-   Dentro do bloco as instruções são executadas contra esta nova base
    selecionada.
-   No fim do bloco o Sequel retorna automaticamente para a base em uso
    anteriormente.

Exemplo:

    using (var scope = new SequelScope("conexao")
    using (scope.CreateDatabaseScope("Blizzard")
    {
      ...instrucoes usando a base Blizzard...
      
      using (scope.CreateDatabaseScope("Northwind")
      {
        ...instrucoes usando a base Northwind...
        
        // no fim deste bloco Sequel retorna automaticamente para a base
        // anterior Blizzard.
      }
      
      ...instrucoes usando a base Blizzard...
    }

---
Out/2017  
Guga Coder
